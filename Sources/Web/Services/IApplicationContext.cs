using System;
using System.Collections.Generic;
using System.Text;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IApplicationContext
    {
        string FullUserName { get; }

        Project CurrentProject { get; set; }
    }
}
