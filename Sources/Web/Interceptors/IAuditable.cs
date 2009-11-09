using System;
using System.Collections.Generic;
using System.Text;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Interceptors
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }

        DateTime UpdatedAt { get; set; }
        string UpdatedBy { get; set; }

        string HistoryDescription { get; }
        Project AuditProject { get; }
    }
}
