using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "select-star";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Expected column names in SELECT";
            }
        }

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
            private int _SelectStarExpressionCount = 0;

            public int SelectStarExpressionCount
            {
                get
                {
                    return _SelectStarExpressionCount;
                }

                set
                {
                    _SelectStarExpressionCount = value;
                }
            }

            public override void Visit(SelectStarExpression node)
            {
                SelectStarExpressionCount++;
            }
        }
    }
}