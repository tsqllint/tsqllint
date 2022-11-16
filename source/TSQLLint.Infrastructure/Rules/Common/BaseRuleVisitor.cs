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

        protected BaseRuleVisitor(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public int DynamicSqlStartColumn { get; set; }
        public int DynamicSqlStartLine { get; set; }
        public abstract string RULE_NAME { get; }
        public abstract string RULE_TEXT { get; }
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Off;

        protected virtual int GetLineNumber(TSqlFragment node) => node.StartLine + GetDynamicSqlLineOffset();

        protected virtual int GetLineNumber(TSqlParserToken node) => node.Line + GetDynamicSqlLineOffset();

        private int GetDynamicSqlLineOffset() =>
            DynamicSqlStartLine > 0
                ? DynamicSqlStartLine - 1
                : 0;

        protected virtual int GetColumnNumber(TSqlFragment node) => node.StartColumn + GetDynamicSqlColumnOffset(node);

        protected virtual int GetColumnNumber(TSqlParserToken node) => node.Column + GetDynamicSqlColumnOffset(node);

        protected virtual int GetDynamicSqlColumnOffset(TSqlFragment node) => GetDynamicSqlColumnOffset(node.StartLine);

        protected virtual int GetDynamicSqlColumnOffset(TSqlParserToken node) => GetDynamicSqlColumnOffset(node.Line);

        private int GetDynamicSqlColumnOffset(int line) =>
            DynamicSqlStartLine > 0 && line == 1
                ? DynamicSqlStartColumn
                : 0;

        public virtual void FixViolation(
            List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
        }
    }
}
