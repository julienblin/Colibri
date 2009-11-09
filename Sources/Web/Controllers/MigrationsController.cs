using System;
using CDS.Framework.Tools.Colibri.Web.Services;
using CDS.Framework.Tools.Colibri.Web.Models;
using NHibernate.Expression;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport.Pagination;
using CDS.Framework.Tools.Colibri.Web.Models.Search;
using CDS.Framework.Tools.Colibri.Web.Helpers;
using System.Diagnostics;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class MigrationsController : ApplicationController
    {
        private IApplicationContext appContext;
        private IMigrationExecutor migrationExecutor;
        private IDBMigrationTablesManager migrationTablesManager;

        public MigrationsController(IApplicationContext appContext, IMigrationExecutor migrationExecutor, IDBMigrationTablesManager migrationTablesManager)
        {
            this.appContext = appContext;
            this.migrationExecutor = migrationExecutor;
            this.migrationTablesManager = migrationTablesManager;
        }

        public void Index([DataBind("search")] MigrationSearch search)
        {
            search.Criterions.Add(Migration.Criterions.InProject(appContext.CurrentProject));
            PropertyBag["search"] = search;
            PropertyBag["inProductionFilterTypes"] = typeof(InProductionFilterType);
            PropertyBag["updatedByList"] = Migration.UpdatedByList(true);

            PropertyBag["migrations"] = ARPaginationHelper.CreatePagination(
                    20,
                    typeof(Migration),
                    new Order[] { OrderHelper.CreateOrder("CreatedAt", false) },
                    search.SearchCriterions()
                );
        }

        public void Show([ARFetch("id", Required=true)] Migration migration)
        {
            PropertyBag["migration"] = migration;
        }

        public void Lines([ARFetch("id", Required = true)] Migration migration)
        {
            PropertyBag["migration"] = migration;
            RenderView("lines", true);
        }

        public void New()
        {
            Migration migration = new Migration();
            migration.Project = appContext.CurrentProject;
            PropertyBag["migration"] = migration;
        }

        [AccessibleThrough(Verb.Post)]
        public void Create([DataBind("migration", Validate = true)] Migration migration)
        {
            if (HasValidationError(migration))
            {
                Flash["error"] = GetErrorSummaryMsg(migration);
                PropertyBag["migration"] = migration;
                RenderView("new");
            }
            else
            {
                migration.Create();
                Flash["message"] = string.Format("Succesfully created migration {0}", migration.Id);
                RedirectToAction("index");
            }
        }

        public void Edit([ARFetch("id", Required = true)] Migration migration)
        {
            PropertyBag["migration"] = migration;
        }

        [AccessibleThrough(Verb.Post)]
        public void Update([ARDataBind("migration", Validate = true, AutoLoad = AutoLoadBehavior.Always)] Migration migration)
        {
            if (HasValidationError(migration))
            {
                Flash["error"] = GetErrorSummaryMsg(migration);
                PropertyBag["migration"] = migration;
                RenderView("edit");
            }
            else
            {
                migration.Update();
                Flash["message"] = string.Format("Succesfully updated migration {0}", migration.Id);
                RedirectToAction("index");
            }
        }

        [AccessibleThrough(Verb.Post)]
        public void Delete([ARFetch("id", Required = true)] Migration migration)
        {
            migration.Delete();
            Flash["message"] = string.Format("Succesfully deleted migration {0}", migration.Id);
            RedirectToAction("index");
        }

        public void SelectInstance([ARFetch("id", Required = true)] Migration migration, ExecutionAction action)
        {
            PropertyBag["migration"] = migration;
            PropertyBag["instances"] = Instance.FindAll(
                    new Order[] { new Order("ServerName", true), new Order("DatabaseName", true) },
                    Instance.Criterions.InProject(migration.Project)
                );
            PropertyBag["action"] = action.ToString();
        }

        public void IsMigrationApplicable([ARFetch("migrationId", Required = true)] Migration migration, [ARFetch("instanceId", Required = true)] Instance instance)
        {
            try
            {
                migrationTablesManager.CheckAndCreateMigrationTable(instance, new Execution());
                if (migrationTablesManager.IsMigrationAlreadyApplied(migration, instance, new Execution()))
                {
                    RenderText("NO");
                }
                else
                {
                    RenderText("YES");
                }
            }
            catch (Exception)
            {
                RenderText("ERROR");
            }
        }

        public void IsMigrationRollbackable([ARFetch("migrationId", Required = true)] Migration migration, [ARFetch("instanceId", Required = true)] Instance instance)
        {
            try
            {
                migrationTablesManager.CheckAndCreateMigrationTable(instance, new Execution());
                if (!migrationTablesManager.IsMigrationAlreadyApplied(migration, instance, new Execution()))
                {
                    RenderText("NO");
                }
                else
                {
                    RenderText("YES");
                }
            }
            catch (Exception)
            {
                RenderText("ERROR");
            }
        }

        public void Execute([ARFetch("migrationId", Required = true)] Migration migration, [ARFetch("instanceId", Required = true)] Instance instance, ExecutionAction action)
        {
            PropertyBag["migration"] = migration;
            PropertyBag["instance"] = instance;
            switch (action)
            {
                case ExecutionAction.Apply:
                    PropertyBag["execution"] = migrationExecutor.AsyncBeginApply(migration, instance);
                    break;
                case ExecutionAction.Rollback:
                    PropertyBag["execution"] = migrationExecutor.AsyncBeginRollback(migration, instance);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }

    public enum ExecutionAction
    {
        Apply,
        Rollback
    }
}
