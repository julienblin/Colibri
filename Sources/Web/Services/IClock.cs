using System;
using System.Collections.Generic;
using System.Text;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IClock
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}
