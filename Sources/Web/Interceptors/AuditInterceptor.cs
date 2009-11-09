using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NHibernate;
using Castle.MicroKernel;
using CDS.Framework.Tools.Colibri.Web.Services;
using CDS.Framework.Tools.Colibri.Web.Models;

namespace CDS.Framework.Tools.Colibri.Web.Interceptors
{
    public class AuditInterceptor : EmptyInterceptor
    {
        private IKernel kernel;

        public AuditInterceptor(IKernel kernel)
        {
            this.kernel = kernel;
        }

       public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            IAuditable auditable = entity as IAuditable;
            if (auditable != null)
            {
                IClock clock = kernel.Resolve<IClock>();
                IApplicationContext appContext = kernel.Resolve<IApplicationContext>();

                currentState[Array.IndexOf<string>(propertyNames, "UpdatedAt")] = clock.Now;
                currentState[Array.IndexOf<string>(propertyNames, "UpdatedBy")] = appContext.FullUserName;

                Project auditProject = auditable.AuditProject;
                if (auditProject != null)
                {
                    ProjectHistoryStep historyStep = new ProjectHistoryStep();
                    historyStep.Description = string.Format("Updated {0}", auditable.HistoryDescription);
                    historyStep.At = clock.Now;
                    historyStep.By = appContext.FullUserName;
                    historyStep.Project = auditProject;
                    historyStep.Create();
                }

                return true;
            }
            return false;
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            IAuditable auditable = entity as IAuditable;
            if (auditable != null)
            {
                IClock clock = kernel.Resolve<IClock>();
                IApplicationContext appContext = kernel.Resolve<IApplicationContext>();

                Project auditProject = auditable.AuditProject;
                if (auditProject != null)
                {
                    ProjectHistoryStep historyStep = new ProjectHistoryStep();
                    historyStep.Description = string.Format("Deleted {0}", auditable.HistoryDescription);
                    historyStep.At = clock.Now;
                    historyStep.By = appContext.FullUserName;
                    historyStep.Project = auditProject;
                    historyStep.Create();
                }
            }
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            IAuditable auditable = entity as IAuditable;
            if (auditable != null)
            {
                IClock clock = kernel.Resolve<IClock>();
                IApplicationContext appContext = kernel.Resolve<IApplicationContext>();

                state[Array.IndexOf<string>(propertyNames, "CreatedAt")] = clock.Now;
                state[Array.IndexOf<string>(propertyNames, "CreatedBy")] = appContext.FullUserName;
                state[Array.IndexOf<string>(propertyNames, "UpdatedAt")] = clock.Now;
                state[Array.IndexOf<string>(propertyNames, "UpdatedBy")] = appContext.FullUserName;

                Project auditProject = auditable.AuditProject;
                if (auditProject != null)
                {
                    ProjectHistoryStep historyStep = new ProjectHistoryStep();
                    historyStep.Description = string.Format("Created {0}", auditable.HistoryDescription);
                    historyStep.At = clock.Now;
                    historyStep.By = appContext.FullUserName;
                    historyStep.Project = auditProject;
                    historyStep.Create();
                }

                return true;
            }
            return false;
        }
    }
}
