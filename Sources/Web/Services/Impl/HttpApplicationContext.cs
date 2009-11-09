using System;
using CDS.Framework.Tools.Colibri.Web.Models;
using System.Web;
using System.Security.Principal;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl
{
    public class HttpApplicationContext : IApplicationContext
    {
        #region IApplicationContext Members

        public string FullUserName
        {
            get
            {
                IIdentity identity = HttpContext.Current.User.Identity;
                if (!identity.IsAuthenticated)
                {
                    return "Anonymous";
                }
                return identity.Name;
            }
        }

        public Project CurrentProject
        {
            get
            {
                return (Project)HttpContext.Current.Session["Context.CurrentProject"];
            }
            set
            {
                HttpContext.Current.Session["Context.CurrentProject"] = value;
            }
        }

        #endregion
    }
}
