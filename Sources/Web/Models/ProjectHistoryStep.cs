using System;
using Castle.ActiveRecord;
using NHibernate.Expression;
using Castle.ActiveRecord.Queries;
using System.Collections;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("project_history_steps")]
    public class ProjectHistoryStep : ActiveRecordBase<ProjectHistoryStep>
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
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        private DateTime at;
        [Property(Column = "step_at")]
        public virtual DateTime At
        {
            get { return at; }
            set { at = value; }
        }

        private string by;
        [Property(Column = "step_by")]
        public virtual string By
        {
            get { return by; }
            set { by = value; }
        }

        private Project project;
        [BelongsTo("project_id")]
        public virtual Project Project
        {
            get { return project; }
            set { project = value; }
        }

        public static string[] ByList(bool includeFirstEmpty)
        {
            ActiveRecordCriteriaQuery query = new ActiveRecordCriteriaQuery(
                    typeof(ProjectHistoryStep),
                    DetachedCriteria.For<ProjectHistoryStep>()
                        .SetProjection(Projections.Distinct(Property.ForName("By")))
                        .AddOrder(new Order("By", true))
                );
            ArrayList list = (ArrayList)ActiveRecordMediator<ProjectHistoryStep>.ExecuteQuery(query);

            if (includeFirstEmpty)
            {
                string[] result = new string[list.Count + 1];
                result[0] = "";
                Array.Copy(list.ToArray(), 0, result, 1, list.Count);
                return result;
            }
            else
            {
                string[] result = new string[list.Count];
                Array.Copy(list.ToArray(), 0, result, 0, list.Count);
                return result;
            }
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
