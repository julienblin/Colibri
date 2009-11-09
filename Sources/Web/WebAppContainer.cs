using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;
using Castle.MonoRail.WindsorExtension;
using CDS.Framework.Tools.Colibri.Web.Controllers;
using CDS.Framework.Tools.Colibri.Web.Components;
using CDS.Framework.Tools.Colibri.Web.Services;
using CDS.Framework.Tools.Colibri.Web.Services.Impl;
using CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer;
using CDS.Framework.Tools.Colibri.Web.Filters;
using CDS.Framework.Tools.Colibri.Web.Interceptors;

namespace CDS.Framework.Tools.Colibri.Web
{
    public class WebAppContainer : WindsorContainer
    {
        public WebAppContainer() : base(new XmlInterpreter(new ConfigResource()))
        {
            RegisterFacilities();
            RegisterServices();
            RegisterComponents();
            RegisterControllers();
            RegisterFilters();
        }

        protected void RegisterFacilities()
        {
            AddFacility("rails", new RailsFacility());
            AddFacility("audit", new AuditFacility());
        }

        protected void RegisterServices()
        {
            AddComponent("ApplicationContext", typeof(IApplicationContext), typeof(HttpApplicationContext));
            AddComponent("Clock", typeof(IClock), typeof(SystemClock));
            AddComponent("DBMigrationTablesManager", typeof(IDBMigrationTablesManager), typeof(SqlServerDBMigrationTablesManager));
            AddComponent("ScriptsGenerator", typeof(IScriptsGenerator), typeof(SqlServerScriptsGenerator));
            AddComponent("MigrationExecutor", typeof(IMigrationExecutor), typeof(SqlServerMigrationExecutor));
        }

        protected void RegisterComponents()
        {
            AddComponent("InfoBoxComponent", typeof(InfoBoxComponent));
        }

        protected void RegisterControllers()
        {
            AddComponent("HomeController", typeof(HomeController));
            AddComponent("ProjectsController", typeof(ProjectsController));
            AddComponent("InstancesController", typeof(InstancesController));
            AddComponent("MigrationsController", typeof(MigrationsController));
            AddComponent("MigrationLinesController", typeof(MigrationLinesController));
            AddComponent("ExecutionsController", typeof(ExecutionsController));
        }

        protected void RegisterFilters()
        {
            AddComponent("CurrentProjectFilter", typeof(CurrentProjectFilter));
        }
    }
}
