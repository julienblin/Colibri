using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using System.Text;
using CDS.Framework.Tools.Colibri.Web.Helpers;
using CDS.Framework.Tools.Colibri.Web.Filters;
using System.Reflection;
using Castle.MonoRail.ActiveRecordSupport.Pagination;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    [Helper(typeof(PostHelper)), Helper(typeof(AuditHelper)), Helper(typeof(PageLinksHelper))]
    [Helper(typeof(FormatHelper)), Helper(typeof(OrderHelper)), Helper(typeof(DateHelper))]
    [FilterAttribute(ExecuteEnum.BeforeAction, typeof(CurrentProjectFilter))]
    public abstract class ApplicationController : ARSmartDispatcherController
    {
        private static Version appVersion;

        private static Version AppVersion
        {
            get
            {
                if (appVersion == null)
                {
                    appVersion = Assembly.GetExecutingAssembly().GetName().Version;
                }
                return appVersion;
            }
        }

        protected override void Initialize()
        {
            PropertyBag["version"] = AppVersion.ToString();
            base.Initialize();
        }

        protected string GetErrorSummaryMsg(object obj)
        {
            StringBuilder text = new StringBuilder();
            bool isFirst = true;
            foreach (string str in GetErrorSummary(obj).ErrorMessages)
            {
                if (!isFirst)
                    text.Append("<br/>");
                text.Append(str);
                isFirst = false;
            }
            return text.ToString();
        } 
    }
}
