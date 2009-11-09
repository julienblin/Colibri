using System;
using NHibernate.Expression;
using System.Collections.Generic;

namespace CDS.Framework.Tools.Colibri.Web.Models.Search
{
    public abstract class ModelSearchBase<T> : IModelSearch
    {
        #region IModelSearch Members

        public ICriterion[] SearchCriterions()
        {
            List<ICriterion> result = new List<ICriterion>();
            InternalSearchCriterions(result);
            result.AddRange(Criterions);
            return result.ToArray();
        }

        private IList<ICriterion> criterions = new List<ICriterion>();
        public IList<ICriterion> Criterions
        {
            get { return criterions; }
        }

        #endregion

        protected abstract void InternalSearchCriterions(IList<ICriterion> result);
    }
}
