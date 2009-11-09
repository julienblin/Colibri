using System;
using NHibernate.Expression;
using System.Collections.Generic;


namespace CDS.Framework.Tools.Colibri.Web.Models.Search
{
    public class ProjectHistoryStepSearch : ModelSearchBase<ProjectHistoryStep>
    {
        private string descriptionLike;
        public string DescriptionLike
        {
            get { return descriptionLike; }
            set { descriptionLike = value; }
        }

        public string by;
        public string By
        {
            get { return by; }
            set { by = value; }
        }

        private DateTime? startAt;
        public DateTime? StartAt
        {
            get { return startAt; }
            set { startAt = value; }
        }

        private DateTime? endAt;
        public DateTime? EndAt
        {
            get { return endAt; }
            set { endAt = value; }
        }

        protected override void InternalSearchCriterions(IList<ICriterion> result)
        {
            if (!string.IsNullOrEmpty(DescriptionLike))
            {
                result.Add(Expression.Like("Description", string.Concat("%", DescriptionLike, "%")));
            }

            if (!string.IsNullOrEmpty(By))
            {
                result.Add(Expression.Eq("By", By));
            }

            if (StartAt.HasValue)
            {
                result.Add(Expression.Ge("At", StartAt.Value));
            }

            if (EndAt.HasValue)
            {
                result.Add(Expression.Le("At", EndAt.Value));
            }
        }
    }
}
