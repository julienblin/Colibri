using System;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using CDS.Framework.Tools.Colibri.Web.Models;
using CDS.Framework.Tools.Colibri.Web.Services;
using NHibernate.Expression;

namespace CDS.Framework.Tools.Colibri.Web.Components
{
    public class InfoBoxComponent : ViewComponent
    {
        private IApplicationContext appContext;

        public InfoBoxComponent(IApplicationContext appContext)
        {
            this.appContext = appContext;
        }

        public override void Render()
        {
            PropertyBag["username"] = appContext.FullUserName;
            PropertyBag["projects"] = Project.FindAll(new Order("Name", true));
            if (appContext.CurrentProject != null)
            {
                PropertyBag["currentprojectId"] = appContext.CurrentProject.Id;
            }

            base.Render();
        }
    }
}
