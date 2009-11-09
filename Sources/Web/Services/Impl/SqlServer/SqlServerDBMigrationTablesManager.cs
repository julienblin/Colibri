using System;
using CDS.Framework.Tools.Colibri.Web.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer
{
    public class SqlServerDBMigrationTablesManager : IDBMigrationTablesManager
    {
        private IConnectionStringProvider connectionStringProvider;

        public SqlServerDBMigrationTablesManager(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        #region IDBMigrationTablesManager Members

        public void CheckAndCreateMigrationTable(Instance instance, Execution currentExecution)
        {
            currentExecution.AppendLog("Checking migration tables on {0}...", instance.FullName);

            using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*)
                                                         FROM sys.objects
                                                         WHERE name='DBAppliedMigrations'
                                                         AND type_desc = 'USER_TABLE'", connection))
                {
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                    {
                        currentExecution.AppendLog("No migrations tables detected. Installing...", instance.FullName);
                        cmd.CommandText = @"CREATE TABLE [DBAppliedMigrations](
	                                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                                            [Executed_at] [datetime] NULL,
	                                            [Executed_by] [nvarchar](255) NULL,
	                                            [MigrateScript] [ntext] NULL,
	                                            [RollbackScript] [ntext] NULL,
	                                            [Migration_id] [int] NULL,
                                                PRIMARY KEY CLUSTERED 
                                                (
	                                                [Id] ASC
                                                )
                                            )";
                        cmd.ExecuteNonQuery();
                        currentExecution.AppendLog("Migrations tables installed.", instance.FullName);
                    }
                    else
                    {
                        currentExecution.AppendLog("Migration tables already installed on {0}.", instance.FullName);
                    }
                }
            }
        }

        public bool IsMigrationAlreadyApplied(Migration migration, Instance instance, Execution currentExecution)
        {
            currentExecution.AppendLog("Is migration {0} already applied on {1} ? ...", migration.Id, instance.FullName);
            bool returnValue = true;
            using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*)
                                                         FROM [DBAppliedMigrations]
                                                         WHERE [Migration_id]=@migrationId", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("migrationId", migration.Id));
                    int applied = (int)cmd.ExecuteScalar();
                    if (applied == 0)
                    {
                        currentExecution.AppendLog("Migration {0} has never been applied on {1}.", migration.Id, instance.FullName);
                        returnValue = false;
                    }
                    else
                    {
                        currentExecution.AppendLog("Migration {0} has already been applied on {1}.", migration.Id, instance.FullName);
                    }
                }
            }
            return returnValue;
        }

        public void WriteSuccessfullMigration(DBAppliedMigration appliedMigration, System.Data.IDbConnection openConnection)
        {
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO [DBAppliedMigrations]
                                                           ([Executed_at]
                                                           ,[Executed_by]
                                                           ,[MigrateScript]
                                                           ,[RollbackScript]
                                                           ,[Migration_id])
                                                     VALUES
                                                           (@executedAt
                                                           ,@executedBy
                                                           ,@migrateScript
                                                           ,@rollbackScript
                                                           ,@migrationId)", (SqlConnection)openConnection))
            {
                cmd.Parameters.Add(new SqlParameter("executedAt", appliedMigration.At));
                cmd.Parameters.Add(new SqlParameter("executedBy", appliedMigration.By));
                cmd.Parameters.Add(new SqlParameter("migrateScript", appliedMigration.MigrateScript));
                cmd.Parameters.Add(new SqlParameter("rollbackScript", appliedMigration.RollbackScript ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("migrationId", appliedMigration.MigrationId));
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSuccessfullRollback(Migration migration, System.Data.IDbConnection openConnection)
        {
            using (SqlCommand cmd = new SqlCommand(@"DELETE FROM [DBAppliedMigrations]
                                                     WHERE [Migration_id]=@migrationId", (SqlConnection)openConnection))
            {
                cmd.Parameters.Add(new SqlParameter("migrationId", migration.Id));
                cmd.ExecuteNonQuery();
            }
        }

        public string GetRollbackstring(Migration migration, Instance instance, Execution currentExecution)
        {
            currentExecution.AppendLog("Retreiving rollback script ...");
            string returnValue = null;
            using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT [RollbackScript]
                                                         FROM [DBAppliedMigrations]
                                                         WHERE [Migration_id]=@migrationId", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("migrationId", migration.Id));
                    returnValue = (string)cmd.ExecuteScalar();
                }
            }
            return returnValue;
        }

        public DBAppliedMigration[] GetAppliedMigrations(Instance instance)
        {
            List<DBAppliedMigration> result = new List<DBAppliedMigration>();
            using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                              ,[Executed_at]
                                                              ,[Executed_by]
                                                              ,[MigrateScript]
                                                              ,[RollbackScript]
                                                              ,[Migration_id]
                                                          FROM [DBAppliedMigrations]
                                                          ORDER BY [Executed_at] DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBAppliedMigration appliedMig = new DBAppliedMigration();
                            appliedMig.Id = reader.GetInt32(0);
                            appliedMig.At = reader.GetDateTime(1);
                            appliedMig.By = reader.GetString(2);
                            appliedMig.MigrateScript = reader.GetString(3);
                            appliedMig.RollbackScript = reader.GetString(4);
                            appliedMig.MigrationId = reader.GetInt32(5);
                            result.Add(appliedMig);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        #endregion
    }
}
