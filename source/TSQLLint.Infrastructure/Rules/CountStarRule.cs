using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class CountStarRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public CountStarRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "count-star";

        public string RULE_TEXT => "COUNT(*) disallowed. Suggest COUNT(1) or COUNT(<PK>)";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }
        public override void Visit(FunctionCall node)
        {
            var functionName = node.FunctionName?.Value;
            if (functionName == null || !functionName.ToUpper().Equals("COUNT"))
            {
                return;
            }
            foreach (ScalarExpression param in node.Parameters)
            {
                var paramVisitor = new ParameterVisitor();
                param.Accept(paramVisitor);
                if (paramVisitor.IsWildcard)
                {
                    errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
                }

            }
        }

        class ParameterVisitor : TSqlFragmentVisitor
        {
            public bool IsWildcard { get; private set; }
            public ParameterVisitor()
            {

                this.IsWildcard = false;
            }
            public override void Visit(ColumnReferenceExpression node)
            {
                this.IsWildcard = node.ColumnType.Equals(ColumnType.Wildcard);
            }
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
