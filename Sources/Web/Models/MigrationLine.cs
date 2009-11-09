using System;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using Castle.ActiveRecord.Queries;
using NHibernate.Expression;
using CDS.Framework.Tools.Colibri.Web.Validators;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("migration_lines")]
    public class MigrationLine : AuditableBase<MigrationLine>
    {
        private int id;
        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int lineOrder = Int32.MinValue;
        [Property]
        public virtual int LineOrder
        {
            get { return lineOrder; }
            set { lineOrder = value; }
        }

        private string migrateScript;
        [Property(ColumnType = "StringClob", SqlType = "ntext")]
        [ValidateNonEmpty("MigrateScript is a required field")]
        public virtual string MigrateScript
        {
            get { return migrateScript; }
            set { migrateScript = value; }
        }

        public string ShortenMigrateScript
        {
            get
            {
                if (MigrateScript.Length > 50)
                {
                    return string.Concat(MigrateScript.Substring(0, 50), "...");
                }
                else
                {
                    return MigrateScript;
                }
            }
        }

        private string rollbackScript;
        [Property(ColumnType = "StringClob", SqlType = "ntext")]
        public virtual string RollbackScript
        {
            get { return rollbackScript; }
            set { rollbackScript = value; }
        }

        private DBObjectType objectType = DBObjectType.None;
        [Property]
        [ValidateNotSameEnumValue(typeof(DBObjectType), DBObjectType.None, "An object type must be selected")]
        public virtual DBObjectType ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }

        private string objectName;
        [Property]
        [ValidateNonEmpty("ObjectName is a required field")]
        public virtual string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        private DBAction action = DBAction.None;
        [Property]
        [ValidateNotSameEnumValue(typeof(DBAction), DBAction.None, "An action must be selected")]
        public virtual DBAction Action
        {
            get { return action; }
            set { action = value; }
        }

        private Migration migration;
        [BelongsTo("migration_id")]
        public virtual Migration Migration
        {
            get { return migration; }
            set { migration = value; }
        }

        public override Project AuditProject
        {
            get
            {
                if (Migration != null)
                {
                    if (Migration.AuditProject == null)
                    {
                        Migration.Refresh(Migration); // Make sure that Migration is hydrated, not lazy loaded.
                    }
                    return Migration.AuditProject;
                }
                return null;
            }
        }

        public override string HistoryDescription
        {
            get { return string.Format("migration line {0} ({1} - {2}) on migration {3}", Id, Action, ObjectName, Migration.Id); }
        }

        public override void Create()
        {
            if (LineOrder == Int32.MinValue)
            {
                LineOrder = MaxOrder(Migration) + 1;
            }
            base.Create();
        }

        public MigrationLine FindPrevious()
        {
            ScalarProjectionQuery<MigrationLine, int> queryMaxPreviousOrder = new ScalarProjectionQuery<MigrationLine, int>(
                        Projections.Max("LineOrder"),
                        Expression.Eq("Migration.Id", Migration.Id),
                        Expression.Lt("LineOrder", LineOrder)
                    );
            int maxPreviousOrder = queryMaxPreviousOrder.Execute();
            return FindFirst(Expression.Eq("LineOrder", maxPreviousOrder));
        }

        public MigrationLine FindNext()
        {
            ScalarProjectionQuery<MigrationLine, int> queryMinNextOrder = new ScalarProjectionQuery<MigrationLine, int>(
                        Projections.Min("LineOrder"),
                        Expression.Eq("Migration.Id", Migration.Id),
                        Expression.Gt("LineOrder", LineOrder)
                    );
            int minNextOrder = queryMinNextOrder.Execute();
            return FindFirst(Expression.Eq("LineOrder", minNextOrder));
        }

        public static int MaxOrder(Migration migration)
        {
            if (CountInMigration(migration) == 0)
            {
                return 0;
            }
            else
            {
                ScalarProjectionQuery<MigrationLine, int> query = new ScalarProjectionQuery<MigrationLine, int>(
                        Projections.Max("LineOrder"),
                        Expression.Eq("Migration.Id", migration.Id)
                    );
                return query.Execute();
            }
        }

        public static int CountInMigration(Migration migration)
        {
            ScalarProjectionQuery<MigrationLine, int> query = new ScalarProjectionQuery<MigrationLine, int>(
                    Projections.Count("Id"),
                    Expression.Eq("Migration.Id", migration.Id)
                );
            return query.Execute();
        }
    }

    public enum DBObjectType
    {
        None,
        StoredProcedure,
        Table,
        View,
        Index,
        Datafix,
        Misc
    }

    public enum DBAction
    {
        None,
        Create,
        Alter,
        Drop,
        Datafix,
        Misc
    }
}
