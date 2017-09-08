using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class ObjectPropertyRule : TSqlFragmentVisitor, ISqlRule
    {
        public ObjectPropertyRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "object-property"; }
        }

        public string RuleText
        {
            get { return "Expected use of SYS.COLUMNS rather than ObjectProperty function"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("OBJECTPROPERTY", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
            }
        }
    }
}