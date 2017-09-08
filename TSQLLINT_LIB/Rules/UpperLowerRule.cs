using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class UpperLowerRule : TSqlFragmentVisitor, ISqlRule
    {
        public UpperLowerRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "upper-lower"; }
        }

        public string RuleText
        {
            get { return "Use of the UPPER or LOWER functions is not required when running database in case insensitive mode"; }
        }
        
        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("UPPER", StringComparison.OrdinalIgnoreCase) ||
                node.FunctionName.Value.Equals("LOWER", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
            }
        }
    }
}