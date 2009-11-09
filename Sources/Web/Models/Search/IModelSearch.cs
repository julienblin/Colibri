using System;
using NHibernate.Expression;
using System.Collections.Generic;

namespace CDS.Framework.Tools.Colibri.Web.Models.Search
{
    public interface IModelSearch
    {
        ICriterion[] SearchCriterions();

        IList<ICriterion> Criterions { get; }
    }
}
