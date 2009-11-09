using System;
using Castle.MonoRail.Framework;
using CDS.Framework.Tools.Colibri.Web.Services;
using CDS.Framework.Tools.Colibri.Web.Models;
using CDS.Framework.Tools.Colibri.Web.Controllers;

namespace CDS.Framework.Tools.Colibri.Web.Filters
{
    public class CurrentProjectFilter : IFilter
    {
        private IApplicationContext appContext;

        public CurrentProjectFilter(IApplicationContext appContext)
        {
            this.appContext = appContext;
        }

        #region IFilter Members

        public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
        {
            if ((appContext.CurrentProject == null)
                && !((controller is ProjectsController) && (controller.Action.Equals("new") || controller.Action.Equals("create")))) // Avoid infinite loop when redirecting
            {
                if (Project.Exists())
                {
                    appContext.CurrentProject = Project.FindFirst();
                }
                else
                {
                    context.Response.Redirect("projects", "new");
                    return false;
                }
            }
            controller.PropertyBag["currentproject"] = appContext.CurrentProject;
            return true;
        }

        #endregion
    }
}
