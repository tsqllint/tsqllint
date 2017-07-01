using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLINT_LIB.Rules
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "select-star"; } }
        public string RULE_TEXT { get { return "Specify column names in SELECT"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public SelectStarRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SelectStarExpression node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }
    }
}