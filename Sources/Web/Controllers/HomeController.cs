using System;
using Castle.MonoRail.ActiveRecordSupport;
using CDS.Framework.Tools.Colibri.Web.Models;
using CDS.Framework.Tools.Colibri.Web.Services;
using Castle.ActiveRecord.Queries;
using NHibernate.Expression;
using Castle.ActiveRecord;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        private IApplicationContext appContext;

        public HomeController(IApplicationContext appContext)
        {
            this.appContext = appContext;
        }

        public void Index()
        {
            PropertyBag["numinstances"] = Instance.CountInProject(appContext.CurrentProject);
            PropertyBag["nummigrations"] = Migration.CountInProject(appContext.CurrentProject);
            PropertyBag["lasthistory"] = ProjectHistoryStep.SlicedFindAll(0, 10,
                    new Order[] { new Order("At", false) },
                    ProjectHistoryStep.Criterions.InProject(appContext.CurrentProject)
                );
        }

        public void ChangeProject([ARFetch("id", Required = true)] Project project, string redirectTo)
        {
            appContext.CurrentProject = project;
            Redirect(redirectTo);
        }
    }
}
