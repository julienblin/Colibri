using System;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using System.Collections;
using NHibernate.Expression;
using Castle.ActiveRecord.Queries;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("migrations")]
    public class Migration : AuditableBase<Migration>
    {
        private int id;
        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string description;
        [Property(ColumnType = "StringClob", SqlType="ntext")]
        [ValidateNonEmpty("Description is a required field")]
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        private DateTime? appliedInProductionAt;
        [Property]
        public virtual DateTime? AppliedInProductionAt
        {
            get { return appliedInProductionAt; }
            set { appliedInProductionAt = value; }
        }

        private Project project;
        [BelongsTo("project_id")]
        public virtual Project Project
        {
            get { return project; }
            set { project = value; }
        }

        public override Project AuditProject
        {
            get { return Project; }
        }

        private IList lines = new ArrayList();
        [HasMany(typeof(MigrationLine), Cascade = ManyRelationCascadeEnum.Delete, OrderBy="LineOrder")]
        public virtual IList Lines
        {
            get { return lines; }
            set { lines = value; }
        }

        private IList executions = new ArrayList();
        [HasMany(typeof(Execution), Cascade = ManyRelationCascadeEnum.Delete, Lazy = true, Inverse = true)]
        public virtual IList Executions
        {
            get { return executions; }
            set { executions = value; }
        }

        public override string HistoryDescription
        {
            get { return string.Format("migration {0} ({1})", Id, Description.Length > 15 ? Description.Substring(15) : Description); }
        }

        public bool CanApplyTo(Instance instance)
        {
            Execution[] instancePending = Execution.FindAll(
                    Execution.Criterions.HasInstance(instance),
                    Execution.Criterions.Pending()
                );

            return (instancePending.Length == 0);
        }

        public static int CountInProject(Project project)
        {
            if (project == null)
                return 0;

            CountQuery countInstances = new CountQuery(typeof(Migration), new ICriterion[] { Expression.Eq("Project.Id", project.Id) });
            return (int)ActiveRecordMediator.ExecuteQuery(countInstances);
        }

        public class Criterions
        {
            public static ICriterion InProject(Project project)
            {
                return Expression.Eq("Project.Id", project.Id);
            }
        }
    }
}
