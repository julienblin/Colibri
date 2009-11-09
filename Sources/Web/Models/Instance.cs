using System;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using Castle.ActiveRecord.Queries;
using NHibernate.Expression;
using System.Collections;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("instances")]
    public class Instance : AuditableBase<Instance>
    {
        private int id;
        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string serverName;
        [Property]
        [ValidateNonEmpty("ServerName is a required field")]
        public virtual string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        private string databaseName;
        [Property]
        [ValidateNonEmpty("DatabaseName is a required field")]
        public virtual string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        private InstanceType instanceType = InstanceType.None;
        [Property]
        public virtual InstanceType InstanceType
        {
            get { return instanceType; }
            set { instanceType = value; }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} - {1}", ServerName, DatabaseName);
            }
        }

        private Project project;
        [BelongsTo("project_id")]
        public virtual Project Project
        {
            get { return project; }
            set { project = value; }
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
            get { return string.Format("instance {0} - {1}", ServerName, DatabaseName); }
        }

        public override Project AuditProject
        {
            get { return Project; }
        }

        public static int CountInProject(Project project)
        {
            if (project == null)
                return 0;

            CountQuery countInstances = new CountQuery(typeof(Instance), new ICriterion[] { Expression.Eq("Project.Id", project.Id) });
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

    public enum InstanceType
    {
        None,
        Development,
        Production
    }
}
