using System;
using CDS.Framework.Tools.Colibri.Web.Services;
using CDS.Framework.Tools.Colibri.Web.Models;
using NHibernate.Expression;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport.Pagination;
using CDS.Framework.Tools.Colibri.Web.Models.Search;
using CDS.Framework.Tools.Colibri.Web.Helpers;
using System.Diagnostics;

namespace CDS.Framework.Tools.Colibri.Web.Controllers
{
    public class MigrationLinesController : ApplicationController
    {
        private IApplicationContext appContext;
        private IScriptsGenerator scriptsGenerator;

        public MigrationLinesController(IApplicationContext appContext, IScriptsGenerator scriptsGenerator)
        {
            this.appContext = appContext;
            this.scriptsGenerator = scriptsGenerator;
        }

        public void Show([ARFetch("id", Required = true)] MigrationLine line)
        {
            PropertyBag["line"] = line;
            RenderView("show", true);
        }

        public void New([ARFetch("migrationId")] Migration migration)
        {
            MigrationLine newLine = new MigrationLine();
            newLine.Migration = migration;
            PropertyBag["line"] = newLine;
            PropertyBag["dbObjectTypes"] = typeof(DBObjectType);
            PropertyBag["dbActions"] = typeof(DBAction);
            RenderView("new", true);
        }

        [AccessibleThrough(Verb.Post)]
        public void Create([DataBind("line", Validate = true)] MigrationLine line)
        {
            if (HasValidationError(line))
            {
                Flash["error"] = GetErrorSummaryMsg(line);
                PropertyBag["line"] = line;
                PropertyBag["dbObjectTypes"] = typeof(DBObjectType);
                PropertyBag["dbActions"] = typeof(DBAction);
                RenderView("new", true);
            }
            else
            {
                line.Create();
                CancelView();
            }
        }

        public void Edit([ARFetch("id", Required = true)] MigrationLine line)
        {
            PropertyBag["line"] = line;
            PropertyBag["dbObjectTypes"] = typeof(DBObjectType);
            PropertyBag["dbActions"] = typeof(DBAction);
            RenderView("edit", true);
        }

        [AccessibleThrough(Verb.Post)]
        public void Update([ARDataBind("line", Validate = true, AutoLoad = AutoLoadBehavior.Always)] MigrationLine line)
        {
            if (HasValidationError(line))
            {
                Flash["error"] = GetErrorSummaryMsg(line);
                PropertyBag["line"] = line;
                PropertyBag["dbObjectTypes"] = typeof(DBObjectType);
                PropertyBag["dbActions"] = typeof(DBAction);
                RenderView("edit", true);
            }
            else
            {
                line.Update();
                CancelView();
            }
        }

        [AccessibleThrough(Verb.Post)]
        public void Delete([ARFetch("id", Required = true)] MigrationLine line)
        {
            line.Delete();
            CancelView();
        }

        public void MoveLine([ARFetch("id", Required = true)] MigrationLine line, MoveLineDirection direction)
        {
            MigrationLine otherLine = null;
            switch (direction)
            {
                case MoveLineDirection.up:
                    otherLine = line.FindPrevious();
                    break;
                case MoveLineDirection.down:
                    otherLine = line.FindNext();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            int oldOtherLineOrder = otherLine.LineOrder;
            otherLine.LineOrder = line.LineOrder;
            line.LineOrder = oldOtherLineOrder;
            otherLine.Update();
            line.Update();
            CancelView();
        }

        public void Instructions(DBObjectType objectType, DBAction action)
        {
            RenderText(scriptsGenerator.InstructionsForAutomaticRollback(objectType, action));
        }
    }

    public enum MoveLineDirection {
        up,
        down
    }
}
