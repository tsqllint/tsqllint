using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "select-star";

        public string RULE_TEXT => "Expected column names in SELECT";

        private readonly Action<string, string, int, int> ErrorCallback;

        private int expressionCounter;

        public SelectStarRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

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

            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
