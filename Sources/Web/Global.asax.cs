using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Castle.Windsor;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web
{
    public class Global : System.Web.HttpApplication, IContainerAccessor
    {
        private static WebAppContainer container;

        public IWindsorContainer Container
        { 
            get { return container; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            container = new WebAppContainer();

            ActiveRecordStarter.Initialize(typeof(Project).Assembly, ActiveRecordSectionHandler.Instance);

            // If you want to let ActiveRecord create the schema for you:
            #if DEBUG
            //ActiveRecordStarter.CreateSchema();
            #endif
        }

        protected void Application_End(object sender, EventArgs e)
        {
            container.Dispose();
        }
    }
}