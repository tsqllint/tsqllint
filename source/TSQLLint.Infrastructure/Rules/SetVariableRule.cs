using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetVariableRule : BaseRuleVisitor, ISqlRule
    {
        public SetVariableRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-variable";

        public override string RULE_TEXT => "Expected variable to be set using SELECT statement";

        public override void Visit(SetVariableStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
