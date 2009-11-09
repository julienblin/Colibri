using System;
using System.Collections.Generic;
using System.Text;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Services
{
    public interface IScriptsGenerator
    {
        string GenerateMigrateScript(Migration migration, Instance instance);
        string GenerateRollbackScript(Migration migration, Instance instance);

        string InstructionsForAutomaticRollback(DBObjectType objectType, DBAction action);
    }
}
