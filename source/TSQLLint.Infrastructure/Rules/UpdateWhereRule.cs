using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class UpdateWhereRule : BaseRuleVisitor, ISqlRule
    {
        public UpdateWhereRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "update-where";

        public override string RULE_TEXT => "Expected WHERE clause for UPDATE statement";


        public override void Visit(UpdateSpecification node)
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
