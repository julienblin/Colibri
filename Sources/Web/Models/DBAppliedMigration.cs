using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    /// <summary>
    /// Saved in destination database... Not an active record object.
    /// </summary>
    public class DBAppliedMigration
    {
        public DBAppliedMigration()
        {
        }

        public DBAppliedMigration(Execution execution)
        {
            At = execution.At;
            By = execution.By;
            MigrationId = execution.Migration.Id;
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime at;
        public DateTime At
        {
            get { return at; }
            set { at = value; }
        }

        private string by;
        public string By
        {
            get { return by; }
            set { by = value; }
        }

        private string migrateScript;
        public string MigrateScript
        {
            get { return migrateScript; }
            set { migrateScript = value; }
        }

        private string rollbackScript;
        public string RollbackScript
        {
            get { return rollbackScript; }
            set { rollbackScript = value; }
        }

        private int migrationId;
        public int MigrationId
        {
            get { return migrationId; }
            set { migrationId = value; }
        }
    }
}
