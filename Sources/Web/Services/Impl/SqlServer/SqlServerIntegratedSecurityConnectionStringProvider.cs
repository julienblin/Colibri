using System;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer
{
    public class SqlServerIntegratedSecurityConnectionStringProvider : IConnectionStringProvider
    {
        #region IConnectionStringProvider Members

        public string GetConnectionString(Instance instance)
        {
            return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", instance.ServerName, instance.DatabaseName);
        }

        #endregion
    }
}
