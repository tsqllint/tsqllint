using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class UpperLowerRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "upper-lower"; } }
        public string RULE_TEXT { get { return "Use of the UPPER or LOWER functions is not required when running database in case insensitive mode."; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public UpperLowerRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("UPPER", StringComparison.OrdinalIgnoreCase) ||
                node.FunctionName.Value.Equals("LOWER", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}