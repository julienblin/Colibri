using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CDS.Framework.Tools.Colibri.Web.Models;
using NHibernate.Expression;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using CDS.Framework.Tools.Colibri.Web.Services;
using Castle.ActiveRecord.Queries;
using Castle.MonoRail.ActiveRecordSupport.Pagination;
using CDS.Framework.Tools.Colibri.Web.Models.Search;
using CDS.Framework.Tools.Colibri.Web.Helpers;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class ProjectsController : ApplicationController
    {
        private IApplicationContext appContext;

        public ProjectsController(IApplicationContext appContext)
        {
            this.appContext = appContext;
        }

        public void Index()
        {
            PropertyBag["projects"] = Project.FindAll(new Order("Name", true));
        }

        public void Show([ARFetch("id", Required = true)] Project project, [DataBind("search")] ProjectHistoryStepSearch search)
        {
            PropertyBag["project"] = project;

            search.Criterions.Add(ProjectHistoryStep.Criterions.InProject(project));
            PropertyBag["search"] = search;
            PropertyBag["byList"] = ProjectHistoryStep.ByList(true);

            PropertyBag["historysteps"] = ARPaginationHelper.CreatePagination(
                20,
                typeof(ProjectHistoryStep),
                new Order[] { OrderHelper.CreateOrder("At", false) },
                search.SearchCriterions()
            );
        }

        public void New()
        {
            PropertyBag["project"] = new Project();
        }

        [AccessibleThrough(Verb.Post)]
        public void Create([DataBind("project", Validate = true)] Project project)
        {
            if (HasValidationError(project))
            {
                Flash["error"] = GetErrorSummaryMsg(project);
                PropertyBag["project"] = project;
                RenderView("new");
            }
            else
            {
                project.Create();
                Flash["message"] = string.Format("Succesfully created project {0}", project.Name);
                appContext.CurrentProject = project;
                Redirect("/home/index.colibri");
            }
        }

        public void Edit([ARFetch("id", Required = true)] Project project)
        {
            PropertyBag["project"] = project;
        }

        [AccessibleThrough(Verb.Post)]
        public void Update([ARDataBind("project", Validate = true, AutoLoad = AutoLoadBehavior.Always)] Project project)
        {
            if (HasValidationError(project))
            {
                Flash["error"] = GetErrorSummaryMsg(project);
                PropertyBag["project"] = project;
                RenderView("edit");
            }
            else
            {
                project.Update();
                Flash["message"] = string.Format("Succesfully updated project {0}", project.Name);
                RedirectToAction("index");
            }
        }

        [AccessibleThrough(Verb.Post)]
        public void Delete([ARFetch("id", Required = true)] Project project)
        {
            project.Delete();
            Flash["message"] = string.Format("Succesfully deleted project {0}", project.Name);
            RedirectToAction("index");
        }
    }
}
