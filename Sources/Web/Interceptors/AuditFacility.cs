using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Castle.MicroKernel;
using Castle.ActiveRecord.Framework;
using NHibernate;

namespace CDS.Framework.Tools.Colibri.Web.Interceptors
{
    public class AuditFacility : IFacility
    {
        #region IFacility Members

        public void Init(IKernel kernel, Castle.Core.Configuration.IConfiguration facilityConfig)
        {
            InterceptorFactory.Create = new InterceptorFactory.CreateInterceptor(delegate()
            {
                return new AuditInterceptor(kernel);
            });
        }

        public void Terminate()
        {
            // Nothing to terminate
        }

        #endregion
    }
}
