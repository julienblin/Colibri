using System;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using System.Collections;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("projects")]
    public class Project : AuditableBase<Project>
    {
        private int id;
        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        [Property]
        [ValidateNonEmpty("Name is a required field")]
        [ValidateIsUnique("Name must be unique")]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        private IList instances = new ArrayList();
        [HasMany(typeof(Instance), Cascade = ManyRelationCascadeEnum.Delete, Lazy = true, Inverse = true)]
        public virtual IList Instances
        {
            get { return instances; }
            set { instances = value; }
        }

        private IList migrations = new ArrayList();
        [HasMany(typeof(Migration), Cascade = ManyRelationCascadeEnum.Delete, Lazy = true, Inverse = true)]
        public virtual IList Migrations
        {
            get { return migrations; }
            set { migrations = value; }
        }

        private IList historySteps = new ArrayList();
        [HasMany(typeof(ProjectHistoryStep), Cascade = ManyRelationCascadeEnum.Delete, Lazy = true, Inverse = true)]
        public virtual IList HistorySteps
        {
            get { return historySteps; }
            set { historySteps = value; }
        }

        public override string HistoryDescription
        {
            get { return string.Format("project {0}", Name); }
        }

        public override Project AuditProject
        {
            get { return null; }
        }
    }
}
