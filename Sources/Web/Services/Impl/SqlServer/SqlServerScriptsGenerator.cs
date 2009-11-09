using System;
using CDS.Framework.Tools.Colibri.Web.Models;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer
{
    public class SqlServerScriptsGenerator : IScriptsGenerator
    {
        private IConnectionStringProvider connectionStringProvider;

        public SqlServerScriptsGenerator(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        #region IScriptsGenerator Members

        public string GenerateMigrateScript(Migration migration, Instance instance)
        {
            StringBuilder result = new StringBuilder();
            foreach (MigrationLine line in migration.Lines)
            {
                if (!string.IsNullOrEmpty(line.MigrateScript))
                {
                    result.AppendLine();
                    result.AppendLine(line.MigrateScript);
                    result.AppendLine();
                    result.AppendLine("GO");
                }
            }
            return result.ToString();
        }

        public string GenerateRollbackScript(Migration migration, Instance instance)
        {
            StringBuilder result = new StringBuilder();
            ArrayList lines = new ArrayList(migration.Lines);
            lines.Reverse();
            foreach (MigrationLine line in lines)
            {
                result.AppendLine();
                if (!string.IsNullOrEmpty(line.RollbackScript))
                {
                    result.AppendLine(line.RollbackScript);
                }
                else
                {
                    switch (line.ObjectType)
                    {
                        case DBObjectType.None:
                            Debug.Assert(false);
                            break;
                        case DBObjectType.StoredProcedure:
                            if (!GenerateRollbackScriptForStoredProcedure(line, instance, result))
                                return null;
                            break;
                        case DBObjectType.Table:
                            if (!GenerateRollbackScriptForTable(line, instance, result))
                                return null;
                            break;
                        case DBObjectType.View:
                            if (!GenerateRollbackScriptForView(line, instance, result))
                                return null;
                            break;
                        default:
                            // No autogenerated rollback -> no rollback
                            return null;
                    }
                }
                result.AppendLine();
                result.AppendLine("GO");
            }
            return result.ToString();
        }

        public string InstructionsForAutomaticRollback(DBObjectType objectType, DBAction action)
        {
            switch (objectType)
            {
                case DBObjectType.StoredProcedure:
                    switch (action)
                    {
                        case DBAction.Create:
                            return "Autogenerated rollback will consist of DROP PROCEDURE [ObjectName];";
                        case DBAction.Alter:
                            return "Autogenerated rollback will save previous version of stored procedure [ObjectName] when applying.";
                        case DBAction.Drop:
                            return "Autogenerated rollback will save previous version of stored procedure [ObjectName] when applying.";
                    }
                    break;
                case DBObjectType.Table:
                    switch (action)
                    {
                        case DBAction.Create:
                            return "Autogenerated rollback will consist of DROP TABLE [ObjectName];";
                    }
                    break;
                case DBObjectType.View:
                    switch (action)
                    {
                        case DBAction.Create:
                            return "Autogenerated rollback will consist of DROP VIEW [ObjectName];";
                    }
                    break;
            }
            return null;
        }

        #endregion

        private bool GenerateRollbackScriptForStoredProcedure(MigrationLine line, Instance instance, StringBuilder result)
        {
            switch (line.Action)
            {
                case DBAction.None:
                    Debug.Assert(false);
                    break;
                case DBAction.Create:
                    result.AppendLine();
                    result.AppendLine(string.Format("-- autogenerated rollback for create procedure {0}", line.ObjectName));
                    result.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC'))", line.ObjectName));
                    result.AppendLine(string.Format("DROP PROCEDURE [{0}];", line.ObjectName));
                    result.AppendLine(string.Format("-- end of autogenerated rollback for create procedure {0}", line.ObjectName));
                    result.AppendLine();
                    return true;
                case DBAction.Alter:
                    result.AppendLine();
                    result.AppendLine(string.Format("-- autogenerated rollback for alter procedure {0}", line.ObjectName));
                    result.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC'))", line.ObjectName));
                    result.AppendLine(string.Format("DROP PROCEDURE [{0}];", line.ObjectName));
                    result.AppendLine("GO");

                    using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_HelpText", connection))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("objname", line.ObjectName));
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    result.AppendLine(reader.GetString(0));
                                }
                            }
                        }
                    }

                    result.AppendLine();
                    result.AppendLine(string.Format("-- end of autogenerated rollback for alter procedure {0}", line.ObjectName));
                    result.AppendLine();
                    return true;
                case DBAction.Drop:
                    result.AppendLine();
                    result.AppendLine(string.Format("-- autogenerated rollback for drop procedure {0}", line.ObjectName));
                    result.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC'))", line.ObjectName));
                    result.AppendLine(string.Format("DROP PROCEDURE [{0}];", line.ObjectName));
                    result.AppendLine("GO");

                    using (SqlConnection connection = new SqlConnection(connectionStringProvider.GetConnectionString(instance)))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_HelpText", connection))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("objname", line.ObjectName));
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    result.AppendLine(reader.GetString(0));
                                }
                            }
                        }
                    }

                    result.AppendLine();
                    result.AppendLine(string.Format("-- end of autogenerated rollback for drop procedure {0}", line.ObjectName));
                    result.AppendLine();
                    return true;
                default:
                    break;
            }
            return false;
        }

        private bool GenerateRollbackScriptForTable(MigrationLine line, Instance instance, StringBuilder result)
        {
            switch (line.Action)
            {
                case DBAction.None:
                    Debug.Assert(false);
                    break;
                case DBAction.Create:
                    result.AppendLine();
                    result.AppendLine(string.Format("-- autogenerated rollback for create table {0}", line.ObjectName));
                    result.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]'))", line.ObjectName));
                    result.AppendLine(string.Format("DROP TABLE [{0}];", line.ObjectName));
                    result.AppendLine(string.Format("-- end of autogenerated rollback for create table {0}", line.ObjectName));
                    result.AppendLine();
                    return true;
                default:
                    break;
            }
            return false;
        }

        private bool GenerateRollbackScriptForView(MigrationLine line, Instance instance, StringBuilder result)
        {
            switch (line.Action)
            {
                case DBAction.None:
                    Debug.Assert(false);
                    break;
                case DBAction.Create:
                    result.AppendLine();
                    result.AppendLine(string.Format("-- autogenerated rollback for create view {0}", line.ObjectName));
                    result.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[{0}]'))", line.ObjectName));
                    result.AppendLine(string.Format("DROP VIEW [{0}];", line.ObjectName));
                    result.AppendLine(string.Format("-- end of autogenerated rollback for create view {0}", line.ObjectName));
                    result.AppendLine();
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}
