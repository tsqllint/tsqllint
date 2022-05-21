using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class CountStarRule : BaseRuleVisitor, ISqlRule
    {
        public CountStarRule(System.Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "count-star";

        public override string RULE_TEXT => "COUNT(*) disallowed. Suggest COUNT(1) or COUNT(<PK>)";

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

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var node = FixHelpers.FindViolatingNode<FunctionCall>(fileLines, ruleViolation);

            foreach (ScalarExpression param in node.Parameters)
            {
                var paramVisitor = new ParameterVisitor();
                param.Accept(paramVisitor);
                if (paramVisitor.IsWildcard)
                {
                    var whileCard = paramVisitor.Expression;
                    actions.RepaceInlineAt(whileCard.StartLine - 1, whileCard.StartColumn - 1, "1");
                }
            }
        }

        private class ParameterVisitor : TSqlFragmentVisitor
        {
            public bool IsWildcard { get; private set; }
            public ColumnReferenceExpression Expression { get; private set; }

            public ParameterVisitor()
            {
                IsWildcard = false;
            }

            public override void Visit(ColumnReferenceExpression node)
            {
                IsWildcard = node.ColumnType.Equals(ColumnType.Wildcard);
                Expression = node;
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
