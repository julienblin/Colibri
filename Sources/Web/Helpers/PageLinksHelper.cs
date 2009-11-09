using System;
using Castle.MonoRail.Framework.Helpers;
using System.Collections;
using System.Text;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class PageLinksHelper : AbstractHelper
    {
        public string Show(IPaginatedPage collection)
        {
            PaginationHelper paginationHelper = new PaginationHelper();
            paginationHelper.SetController(Controller);

            StringBuilder result = new StringBuilder();
            result.AppendFormat("<div class=\"pagination\"><span class=\"pagination-resume\">Showing {0} - {1} of {2}</span>", collection.FirstItem, collection.LastItem, collection.TotalItems);
            if (collection.TotalItems >= collection.PageSize)
            {
                result.Append("<span class=\"pagination-links\">");

                if (collection.HasFirst)
                    result.Append(paginationHelper.CreatePageLinkWithCurrentQueryString(1, "first", null));
                else
                    result.Append("first");

                if (collection.HasPrevious)
                {
                    result.Append(" | ");
                    result.Append(paginationHelper.CreatePageLinkWithCurrentQueryString(collection.PreviousIndex, "prev", null));
                }
                else
                    result.Append(" | prev");

                if (collection.HasNext)
                {
                    result.Append(" | ");
                    result.Append(paginationHelper.CreatePageLinkWithCurrentQueryString(collection.NextIndex, "next", null));
                }
                else
                    result.Append(" | next");

                if (collection.HasLast)
                {
                    result.Append(" | ");
                    result.Append(paginationHelper.CreatePageLinkWithCurrentQueryString(collection.LastIndex, "last", null));
                }
                else
                    result.Append(" | last");

                result.Append("</span>");
            }
            

            result.Append("</div>");
            return result.ToString();
        }
    }
}
