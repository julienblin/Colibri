using System;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IMigrationExecutor
    {
        Execution AsyncBeginApply(Migration migration, Instance instance);
        Execution AsyncBeginRollback(Migration migration, Instance instance);
    }
}
