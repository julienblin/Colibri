using System;
using NHibernate.Expression;
using System.Collections.Generic;


namespace CDS.Framework.Tools.Colibri.Web.Models.Search
{
    public class MigrationSearch : ModelSearchBase<Migration>
    {
        private int? id;
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        private string descriptionLike;
        public string DescriptionLike
        {
            get { return descriptionLike; }
            set { descriptionLike = value; }
        }

        private InProductionFilterType inProduction = InProductionFilterType.Undefined;
        public InProductionFilterType InProduction
        {
            get { return inProduction; }
            set { inProduction = value; }
        }

        public string updatedBy;
        public string UpdatedBy
        {
            get { return updatedBy; }
            set { updatedBy = value; }
        }

        protected override void InternalSearchCriterions(IList<ICriterion> result)
        {
            if (Id.HasValue)
            {
                result.Add(Expression.Eq("Id", Id.Value));
            }

            if (!string.IsNullOrEmpty(DescriptionLike))
            {
                result.Add(Expression.Like("Description", string.Concat("%", DescriptionLike, "%")));
            }

            if (InProduction != InProductionFilterType.Undefined)
            {
                result.Add(InProduction == InProductionFilterType.Yes ? Expression.IsNotNull("AppliedInProductionAt") : Expression.IsNull("AppliedInProductionAt"));
            }

            if (!string.IsNullOrEmpty(UpdatedBy))
            {
                result.Add(Expression.Eq("UpdatedBy", UpdatedBy));
            }
        }
    }

    public enum InProductionFilterType
    {
        Undefined,
        Yes,
        No
    }
}
