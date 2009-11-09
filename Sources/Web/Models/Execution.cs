using System;
using Castle.ActiveRecord;
using NHibernate.Expression;
using System.Text;

namespace CDS.Framework.Tools.Colibri.Web.Models
{
    [ActiveRecord("executions")]
    public class Execution : ActiveRecordBase<Execution>
    {
        private int id;
        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime at;
        [Property(Column = "executed_at")]
        public virtual DateTime At
        {
            get { return at; }
            set { at = value; }
        }

        private string by;
        [Property(Column = "executed_by")]
        public virtual string By
        {
            get { return by; }
            set { by = value; }
        }

        private string executionLog = string.Empty;
        [Property(ColumnType = "StringClob", SqlType="ntext")]
        public virtual string ExecutionLog
        {
            get { return executionLog; }
            set { executionLog = value; }
        }

        private ExecutionState executionState;
        [Property]
        public virtual ExecutionState ExecutionState
        {
            get { return executionState; }
            set { executionState = value; }
        }

        public bool Pending
        {
            get
            {
                return (ExecutionState == ExecutionState.Pending);
            }
        }

        private Migration migration;
        [BelongsTo("migration_id")]
        public virtual Migration Migration
        {
            get { return migration; }
            set { migration = value; }
        }

        private Instance instance;
        [BelongsTo("instance_id")]
        public virtual Instance Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public void AppendLog(string log)
        {
            ExecutionLog += string.Concat(log, Environment.NewLine, Environment.NewLine);
        }

        public void AppendLog(string format, params object[] args)
        {
            AppendLog(string.Format(format, args));
        }

        public void AppendLogException(Exception ex)
        {
            AppendLog(ex.ToString());
            ExecutionState = ExecutionState.OnError;
        }

        public class Criterions
        {
            public static ICriterion HasInstance(Instance instance)
            {
                return Expression.Eq("Instance.Id", instance.Id);
            }

            public static ICriterion Pending()
            {
                return Expression.Eq("ExecutionState", ExecutionState.Pending);
            }
        }

        public string ToJSON()
        {
            return string.Format(@"{{
                ""Id"": ""{0}"",
                ""ExecutionLog"": ""{1}"",
                ""ExecutionState"": ""{2}""
            }}", Id, ExecutionLog.Replace("\"", "\\\"").Replace(Environment.NewLine, "\\n"), ExecutionState);
        }
    }

    public enum ExecutionState
    {
        NotStarted,
        Pending,
        Completed,
        OnError
    }
}
