using System;
using Castle.MonoRail.ActiveRecordSupport;
using CDS.Framework.Tools.Colibri.Web.Models;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport.Pagination;
using CDS.Framework.Tools.Colibri.Web.Helpers;
using CDS.Framework.Tools.Colibri.Web.Models.Search;
using NHibernate.Expression;
using CDS.Framework.Tools.Colibri.Web.Services;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class ExecutionsController : ApplicationController
    {
        private IApplicationContext appContext;

        public ExecutionsController(IApplicationContext appContext)
        {
            this.appContext = appContext;
        }

        public void Get([ARFetch("id")] Execution execution)
        {
            Response.ContentType = "application/json";
            RenderText(execution.ToJSON());
        }

        public void Index([DataBind("search")] ExecutionSearch search)
        {
            PropertyBag["search"] = search;
            PropertyBag["executionStates"] = typeof(ExecutionState);
            PropertyBag["instances"] = Instance.FindAll(
                    new Order[] { new Order("ServerName", true), new Order("DatabaseName", true) },
                    Instance.Criterions.InProject(appContext.CurrentProject)
                );

            PropertyBag["executions"] = ARPaginationHelper.CreatePagination(
                    20,
                    typeof(Execution),
                    new Order[] { OrderHelper.CreateOrder("At", false) },
                    search.SearchCriterions()
                );
        }

        public void Show([ARFetch("id")] Execution execution)
        {
            PropertyBag["execution"] = execution;
        }

        [AccessibleThrough(Verb.Post)]
        public void DeclareStale([ARFetch("id")] Execution execution)
        {
            execution.AppendLog("Manually stopped - declared staled.");
            execution.ExecutionState = ExecutionState.OnError;
            execution.UpdateAndFlush();
            RedirectToAction("index");
        }
    }
}
