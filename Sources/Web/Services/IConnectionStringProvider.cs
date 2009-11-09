using System;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(Instance instance);
    }
}
