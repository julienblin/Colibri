using System;
using Castle.MonoRail.Framework.Helpers;
using CDS.Framework.Tools.Colibri.Web.Interceptors;
using System.Text;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class AuditHelper : AbstractHelper
    {
        public string AuditIcon(IAuditable auditable)
        {
            DateFormatHelper dateHelper = new DateFormatHelper();
            StringBuilder auditInfo = new StringBuilder();
            auditInfo.AppendFormat("Created at: {0}\n", dateHelper.ToShortDateTime(auditable.CreatedAt));
            auditInfo.AppendFormat("Created by: {0}\n", HtmlEncode(auditable.CreatedBy));
            auditInfo.AppendFormat("Updated at: {0}\n", dateHelper.ToShortDateTime(auditable.UpdatedAt));
            auditInfo.AppendFormat("Updated by: {0}\n", HtmlEncode(auditable.UpdatedBy));

            return string.Format("<img src='/Content/images/audit.png' alt='{0}' />", auditInfo.ToString().Replace("'", "\\'"));
        }
    }
}
