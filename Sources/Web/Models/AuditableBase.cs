using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Castle.ActiveRecord;
using CDS.Framework.Tools.Colibri.Web.Interceptors;
using Castle.ActiveRecord.Queries;
using NHibernate.Expression;
using System.Collections;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    public abstract class AuditableBase<T> : ActiveRecordBase<T>, IAuditable where T : class
    {
        #region IAuditable Members

        private DateTime createdAt;
        [Property]
        public virtual DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }

        private string createdBy;
        [Property]
        public virtual string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        private DateTime updatedAt;
        [Property]
        public virtual DateTime UpdatedAt
        {
            get { return updatedAt; }
            set { updatedAt = value; }
        }

        private string updatedBy;
        [Property]
        public virtual string UpdatedBy
        {
            get { return updatedBy; }
            set { updatedBy = value; }
        }

        public abstract string HistoryDescription { get; }
        public abstract Project AuditProject { get; }

        #endregion

        public static string[] CreatedByList(bool includeFirstEmpty)
        {
            return TouchedByList("CreatedBy", includeFirstEmpty);
        }

        public static string[] UpdatedByList(bool includeFirstEmpty)
        {
            return TouchedByList("UpdatedBy", includeFirstEmpty);
        }

        protected static string[] TouchedByList(string propertyName, bool includeFirstEmpty)
        {
            ActiveRecordCriteriaQuery query = new ActiveRecordCriteriaQuery(
                    typeof(T),
                    DetachedCriteria.For<T>()
                        .SetProjection(Projections.Distinct(Property.ForName(propertyName)))
                        .AddOrder(new Order(propertyName, true))
                );
            ArrayList list = (ArrayList)ActiveRecordMediator<T>.ExecuteQuery(query);

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
    }
}
