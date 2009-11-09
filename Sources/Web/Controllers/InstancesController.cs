using System;
using CDS.Framework.Tools.Colibri.Web.Models;
using CDS.Framework.Tools.Colibri.Web.Services;
using Castle.MonoRail.Framework;
using NHibernate.Expression;
using Castle.MonoRail.ActiveRecordSupport;
using System.Diagnostics;
using System.Collections.Generic;
using Castle.MonoRail.Framework.Helpers;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class InstancesController : ApplicationController
    {
        private IApplicationContext appContext;
        private IDBMigrationTablesManager migrationTablesManager;

        public InstancesController(IApplicationContext appContext, IDBMigrationTablesManager migrationTablesManager)
        {
            this.appContext = appContext;
            this.migrationTablesManager = migrationTablesManager;
        }

        public void Index()
        {
            PropertyBag["instances"] = Instance.FindAll(
                    new Order[] { new Order("ServerName", true), new Order("DatabaseName", true) },
                    Instance.Criterions.InProject(appContext.CurrentProject)
                );
        }

        public void Show([ARFetch("id")] Instance instance)
        {
            PropertyBag["instance"] = instance;
            PropertyBag["displayType"] = instance.InstanceType != InstanceType.None;
            PropertyBag["appliedMigrations"] = PaginationHelper.CreatePagination<DBAppliedMigration>(this,
                migrationTablesManager.GetAppliedMigrations(instance),
                20);
        }

        public void New()
        {
            Instance instance = new Instance();
            instance.Project = appContext.CurrentProject;
            PropertyBag["instance"] = instance;
            PropertyBag["instanceTypes"] = typeof(InstanceType);
        }

        [AccessibleThrough(Verb.Post)]
        public void Create([DataBind("instance", Validate = true)] Instance instance)
        {
            if (HasValidationError(instance))
            {
                Flash["error"] = GetErrorSummaryMsg(instance);
                PropertyBag["instance"] = instance;
                PropertyBag["instanceTypes"] = typeof(InstanceType);
                RenderView("new");
            }
            else
            {
                instance.Create();
                Flash["message"] = string.Format("Succesfully created instance {0}", instance.FullName);
                RedirectToAction("index");
            }
        }

        public void Edit([ARFetch("id", Required = true)] Instance instance)
        {
            PropertyBag["instance"] = instance;
            PropertyBag["instanceTypes"] = typeof(InstanceType);
        }

        [AccessibleThrough(Verb.Post)]
        public void Update([ARDataBind("instance", Validate = true, AutoLoad = AutoLoadBehavior.Always)] Instance instance)
        {
            if (HasValidationError(instance))
            {
                Flash["error"] = GetErrorSummaryMsg(instance);
                PropertyBag["instance"] = instance;
                PropertyBag["instanceTypes"] = typeof(InstanceType);
                RenderView("edit");
            }
            else
            {
                instance.Update();
                Flash["message"] = string.Format("Succesfully updated instance {0}", instance.FullName);
                RedirectToAction("index");
            }
        }

        [AccessibleThrough(Verb.Post)]
        public void Delete([ARFetch("id", Required = true)] Instance instance)
        {
            instance.Delete();
            Flash["message"] = string.Format("Succesfully deleted instance {0}", instance.FullName);
            RedirectToAction("index");
        }

        public void SelectCompare()
        {
            PropertyBag["instances"] = Instance.FindAll(
                    new Order[] { new Order("ServerName", true), new Order("DatabaseName", true) },
                    Instance.Criterions.InProject(appContext.CurrentProject)
                );
        }

        public void Compare(int[] ids)
        {
            Debug.Assert(ids.Length == 2);

            Instance instance1 = Instance.Find(ids[0]);
            Instance instance2 = Instance.Find(ids[1]);

            DBAppliedMigration[] migrations1 = migrationTablesManager.GetAppliedMigrations(instance1);
            DBAppliedMigration[] migrations2 = migrationTablesManager.GetAppliedMigrations(instance2);

            PropertyBag["notInInstance1"] = FindMigrationsNotInInstance(migrations1, migrations2);
            PropertyBag["notInInstance2"] = FindMigrationsNotInInstance(migrations2, migrations1);

            PropertyBag["instance1"] = instance1;
            PropertyBag["instance2"] = instance2;
        }

        private static DBAppliedMigration[] FindMigrationsNotInInstance(DBAppliedMigration[] migrations1, DBAppliedMigration[] migrations2)
        {
            List<DBAppliedMigration> result = new List<DBAppliedMigration>();

            foreach (DBAppliedMigration appliedMig2 in migrations2)
            {
                bool found = false;
                foreach (DBAppliedMigration appliedMig1 in migrations1)
                {
                    if (appliedMig1.MigrationId == appliedMig2.MigrationId)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    result.Add(appliedMig2);
                }
            }

            return result.ToArray();
        }
    }
}
