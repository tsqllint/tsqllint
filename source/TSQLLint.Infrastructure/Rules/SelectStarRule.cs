using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SelectStarRule : BaseRuleVisitor, ISqlRule
    {
        private int expressionCounter;

        public SelectStarRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "select-star";

        public override string RULE_TEXT => "Expected column names in SELECT";

        public override void Visit(ExistsPredicate node)
        {
            // count select star expressions in predicate
            var childVisitor = new ChildVisitor();
            node.AcceptChildren(childVisitor);
            expressionCounter += childVisitor.SelectStarExpressionCount;
        }

        public override void Visit(SelectStarExpression node)
        {
            if (expressionCounter > 0)
            {
                expressionCounter--;
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }

        public class ChildVisitor : TSqlFragmentVisitor
        {
            public int SelectStarExpressionCount { get; set; }

            public override void Visit(SelectStarExpression node)
            {
                SelectStarExpressionCount++;
            }
        }
    }
}
