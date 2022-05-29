using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
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

        public virtual void FixViolation(
            List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
        }

        protected void Ignore(List<string> fileLines, FileLineActions actions)
        {
            var disableStatement = $"/* tsqllint-disable {RULE_NAME} - approved by: Damon Ashman */";
            var index = 0;
            while (fileLines[index].StartsWith("--"))
            {
                index++;
            }
            if (!fileLines.Contains(disableStatement))
            {
                actions.Insert(index, disableStatement);
            }
        }
    }
}
