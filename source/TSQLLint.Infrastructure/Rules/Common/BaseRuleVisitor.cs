using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules.Common
{
    public abstract class BaseRuleVisitor : TSqlFragmentVisitor, ISqlRule
    {
        protected readonly Action<string, string, int, int> errorCallback;

        public BaseRuleVisitor(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public int DynamicSqlStartColumn { get; set; }
        public int DynamicSqlStartLine { get; set; }
        public abstract string RULE_NAME { get; }
        public abstract string RULE_TEXT { get; }

        public virtual void FixViolation(string[] fileLines, IRuleViolation ruleViolation)
        {
        }
    }
}
