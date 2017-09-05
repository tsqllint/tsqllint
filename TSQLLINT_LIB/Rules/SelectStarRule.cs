using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        private int _expressionCounter;

        public SelectStarRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "select-star"; }
        }

        public string RuleText
        {
            get { return "Expected column names in SELECT"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(ExistsPredicate node)
        {
            // count select star expressions in predicate
            var childVisitor = new ChildVisitor();
            node.AcceptChildren(childVisitor);
            _expressionCounter += childVisitor.SelectStarExpressionCount;
        }

        public override void Visit(SelectStarExpression node)
        {
            if (_expressionCounter > 0)
            {
                _expressionCounter--;
                return;
            }

            ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
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