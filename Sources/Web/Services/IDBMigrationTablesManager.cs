using System;
using System.Collections.Generic;
using System.Text;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IDBMigrationTablesManager
    {
        void CheckAndCreateMigrationTable(Instance instance, Execution currentExecution);
        bool IsMigrationAlreadyApplied(Migration migration, Instance instance, Execution currentExecution);
        
        void WriteSuccessfullMigration(DBAppliedMigration appliedMigration, System.Data.IDbConnection openConnection);
        void DeleteSuccessfullRollback(Migration migration, System.Data.IDbConnection openConnection);
        
        string GetRollbackstring(Migration migration, Instance instance, Execution currentExecution);

        DBAppliedMigration[] GetAppliedMigrations(Instance instance);
    }
}
