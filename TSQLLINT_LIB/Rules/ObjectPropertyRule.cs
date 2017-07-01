using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLINT_LIB.Rules
{
    public class ObjectPropertyRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "object-property"; } }
        public string RULE_TEXT { get { return "Use sys.columns query instead of objectProperty function"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public ObjectPropertyRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("OBJECTPROPERTY", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}