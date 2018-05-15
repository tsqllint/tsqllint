using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private int expressionCounter;

        public SelectStarRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "select-star";

        public string RULE_TEXT => "Expected column names in SELECT";

        public int DynamicSqlStartColumn { get; set; }

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

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
