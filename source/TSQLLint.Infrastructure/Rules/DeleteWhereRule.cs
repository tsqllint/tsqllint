using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DeleteWhereRule : BaseRuleVisitor, ISqlRule
    {
        public DeleteWhereRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "delete-where";

        public override string RULE_TEXT => "Expected WHERE clause for DELETE statement";

        public override void Visit(DeleteSpecification node)
        {
            if (node.WhereClause != null)
            {
                return;
            }

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
