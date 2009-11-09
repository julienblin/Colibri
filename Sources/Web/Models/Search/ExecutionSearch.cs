using System;
using NHibernate.Expression;
using System.Collections.Generic;


namespace CDS.Framework.Tools.Colibri.Web.Models.Search
{
    public class ExecutionSearch : ModelSearchBase<Migration>
    {
        private int? id;
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        private ExecutionState? executionState;
        public virtual ExecutionState? ExecutionState
        {
            get { return executionState; }
            set { executionState = value; }
        }

        private int? instanceId;
        public int? InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; }
        }

        protected override void InternalSearchCriterions(IList<ICriterion> result)
        {
            if (Id.HasValue)
            {
                result.Add(Expression.Eq("Id", Id.Value));
            }

            if (ExecutionState.HasValue)
            {
                result.Add(Expression.Eq("ExecutionState", ExecutionState.Value));
            }

            if (InstanceId.HasValue)
            {
                result.Add(Expression.Eq("Instance.Id", InstanceId.Value));
            }
        }
    }
}
