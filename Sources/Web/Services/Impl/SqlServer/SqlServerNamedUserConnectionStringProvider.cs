using System;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl.SqlServer
{
    public class SqlServerNamedUserConnectionStringProvider : IConnectionStringProvider
    {
        private string userName;
        private string password;

        public SqlServerNamedUserConnectionStringProvider(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        #region IConnectionStringProvider Members

        public string GetConnectionString(Instance instance)
        {
            return string.Format("Data Source={0},1433;Initial Catalog={1};User ID={2};password={3};", instance.ServerName, instance.DatabaseName, userName, password);
        }

        #endregion
    }
}
