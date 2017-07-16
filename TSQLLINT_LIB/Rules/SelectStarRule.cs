using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "select-star"; } }
        public string RULE_TEXT { get { return "Specify column names in SELECT"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private int expressionCounter;

        public SelectStarRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(ExistsPredicate node)
        {
            var selectStarExpressionsInPredicate = 0;
            for (var index = 0; index < ((QuerySpecification) (node.Subquery.QueryExpression)).SelectElements.Count; index++)
            {
                var x = ((QuerySpecification) (node.Subquery.QueryExpression)).SelectElements[index];
                if (x.GetType() == typeof(SelectStarExpression))
                {
                    selectStarExpressionsInPredicate++;
                }
            }
            expressionCounter += selectStarExpressionsInPredicate;
        }

        public override void Visit(SelectStarExpression node)
        {
            if (expressionCounter > 0)
            {
                expressionCounter--;
                return;
            }

            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }
    }
}