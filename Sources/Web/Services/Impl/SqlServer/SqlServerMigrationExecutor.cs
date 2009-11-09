using System;
using CDS.Framework.Tools.Colibri.Web.Models;
using System.Threading;
using Castle.ActiveRecord;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.IO;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer
{
    public class SqlServerMigrationExecutor : IMigrationExecutor
    {
        private IConnectionStringProvider connectionStringProvider;
        private IDBMigrationTablesManager migrationTablesManager;
        private IClock clock;
        private IApplicationContext appContext;
        private IScriptsGenerator scriptsGenerator;

        public SqlServerMigrationExecutor(IClock clock, IApplicationContext appContext,
            IConnectionStringProvider connectionStringProvider, IDBMigrationTablesManager migrationTablesManager, IScriptsGenerator scriptsGenerator)
        {
            this.clock = clock;
            this.appContext = appContext;
            this.connectionStringProvider = connectionStringProvider;
            this.migrationTablesManager = migrationTablesManager;
            this.scriptsGenerator = scriptsGenerator;
        }

        #region IMigrationExecutor Members

        public Execution AsyncBeginApply(Migration migration, Instance instance)
        {
            ProjectHistoryStep historyStep = new ProjectHistoryStep();
            historyStep.Description = string.Format("Applying migration {0} ({1}) on instance {2}.", migration.Id, migration.Description, instance.FullName);
            historyStep.At = clock.Now;
            historyStep.By = appContext.FullUserName;
            historyStep.Project = migration.AuditProject;
            historyStep.Create();

            Execution execution = new Execution();
            execution.At = clock.Now;
            execution.By = appContext.FullUserName;
            execution.Migration = migration;
            execution.Instance = instance;
            execution.ExecutionState = ExecutionState.Pending;
            execution.AppendLog("Starting migration {0}({1}) on {2}...", migration.Id, migration.Description, instance.FullName);
            execution.CreateAndFlush();
            try
            {
                migrationTablesManager.CheckAndCreateMigrationTable(instance, execution);
                execution.UpdateAndFlush();
                if (!migrationTablesManager.IsMigrationAlreadyApplied(migration, instance, execution))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object arg)
                    {
                        SyncApplyMigration(migration, instance, execution);
                    }));
                }
                else
                {
                    execution.ExecutionState = ExecutionState.Completed;
                    execution.UpdateAndFlush();
                }

            }
            catch (Exception ex)
            {
                execution.AppendLogException(ex);
                execution.UpdateAndFlush();
            }

            return execution;
        }

        public Execution AsyncBeginRollback(Migration migration, Instance instance)
        {
            ProjectHistoryStep historyStep = new ProjectHistoryStep();
            historyStep.Description = string.Format("Rollbacking migration {0} ({1}) on instance {2}.", migration.Id, migration.Description, instance.FullName);
            historyStep.At = clock.Now;
            historyStep.By = appContext.FullUserName;
            historyStep.Project = migration.AuditProject;
            historyStep.Create();

            Execution execution = new Execution();
            execution.At = clock.Now;
            execution.By = appContext.FullUserName;
            execution.Migration = migration;
            execution.Instance = instance;
            execution.ExecutionState = ExecutionState.Pending;
            execution.AppendLog("Starting rollback of {0}({1}) on {2}...", migration.Id, migration.Description, instance.FullName);
            execution.CreateAndFlush();
            try
            {
                migrationTablesManager.CheckAndCreateMigrationTable(instance, execution);
                execution.UpdateAndFlush();
                if (migrationTablesManager.IsMigrationAlreadyApplied(migration, instance, execution))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object arg)
                    {
                        SyncRollbackMigration(migration, instance, execution);
                    }));
                }
                else
                {
                    execution.ExecutionState = ExecutionState.Completed;
                    execution.UpdateAndFlush();
                }

            }
            catch (Exception ex)
            {
                execution.AppendLogException(ex);
                execution.UpdateAndFlush();
            }

            return execution;
        }

        #endregion

        private void SyncApplyMigration(Migration migration, Instance instance, Execution execution)
        {
            try
            {
                execution.AppendLog("Generating rollback scripts...");
                execution.UpdateAndFlush();
                string rollbackScript = scriptsGenerator.GenerateRollbackScript(migration, instance);
                execution.AppendLog(string.Concat(Environment.NewLine, rollbackScript, Environment.NewLine));
                execution.UpdateAndFlush();

                execution.AppendLog("Generating migrate scripts...");
                execution.UpdateAndFlush();
                string migrateScript = scriptsGenerator.GenerateMigrateScript(migration, instance);
                execution.AppendLog(string.Concat(Environment.NewLine, migrateScript, Environment.NewLine));
                execution.UpdateAndFlush();

                execution.AppendLog("Migrating...");
                execution.UpdateAndFlush();

                using (SqlConnection conn = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
                {
                    conn.Open();

                    conn.InfoMessage += new SqlInfoMessageEventHandler(delegate(object sender, SqlInfoMessageEventArgs e)
                    {
                        execution.AppendLog(e.Message);
                        execution.UpdateAndFlush();
                    });

                    using (System.Transactions.TransactionScope transaction = new System.Transactions.TransactionScope())
                    {
                        List<string> sqlStatements = GetSqlStatements(migrateScript);
                        foreach (string statement in sqlStatements)
                        {
                            using (SqlCommand cmd = new SqlCommand(statement, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        execution.AppendLog("Scripts completed. Writing migration audit...");
                        execution.UpdateAndFlush();

                        DBAppliedMigration appliedMigration = new DBAppliedMigration(execution);
                        appliedMigration.MigrateScript = migrateScript;
                        appliedMigration.RollbackScript = rollbackScript;
                        migrationTablesManager.WriteSuccessfullMigration(appliedMigration, conn);

                        transaction.Complete();
                    }
                }

                execution.AppendLog("Migration ended with success!");
                execution.ExecutionState = ExecutionState.Completed;
                execution.UpdateAndFlush();
            }
            catch (Exception ex)
            {
                execution.AppendLogException(ex);
                execution.UpdateAndFlush();
            }
        }

        private void SyncRollbackMigration(Migration migration, Instance instance, Execution execution)
        {
            try
            {
                string rollbackScript = migrationTablesManager.GetRollbackstring(migration, instance, execution);
                if (string.IsNullOrEmpty(rollbackScript))
                {
                    execution.AppendLog("Impossible to rollback! Aborting...");
                    execution.ExecutionState = ExecutionState.OnError;
                    return;
                }

                execution.AppendLog(string.Concat(Environment.NewLine, rollbackScript, Environment.NewLine));
                execution.UpdateAndFlush();

                execution.AppendLog("Rollbacking...");
                execution.UpdateAndFlush();

                using (SqlConnection conn = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
                {
                    conn.Open();

                    conn.InfoMessage += new SqlInfoMessageEventHandler(delegate(object sender, SqlInfoMessageEventArgs e)
                    {
                        execution.AppendLog(e.Message);
                        execution.UpdateAndFlush();
                    });

                    using (System.Transactions.TransactionScope transaction = new System.Transactions.TransactionScope())
                    {
                        List<string> sqlStatements = GetSqlStatements(rollbackScript);
                        foreach (string statement in sqlStatements)
                        {
                            using (SqlCommand cmd = new SqlCommand(statement, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        execution.AppendLog("Scripts completed. Deleting migration entry...");
                        execution.UpdateAndFlush();

                        migrationTablesManager.DeleteSuccessfullRollback(migration, conn);

                        transaction.Complete();
                    }
                }

                execution.AppendLog("Rollback ended with success!");
                execution.ExecutionState = ExecutionState.Completed;
                execution.UpdateAndFlush();
            }
            catch (Exception ex)
            {
                execution.AppendLogException(ex);
                execution.UpdateAndFlush();
            }
        }

        private List<string> GetSqlStatements(string script)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            using(StringReader reader = new StringReader(script))
            {
                string currentLine = reader.ReadLine();
                while (currentLine != null)
                {
                    if (currentLine.StartsWith("GO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(sb.ToString().Trim()))
                        {
                            result.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                    }
                    else
                    {
                        sb.AppendLine(currentLine);
                    }
                    currentLine = reader.ReadLine();
                }
            }

            if (!string.IsNullOrEmpty(sb.ToString().Trim()))
            {
                result.Add(sb.ToString());
            }

            return result;
        }
    }
}
