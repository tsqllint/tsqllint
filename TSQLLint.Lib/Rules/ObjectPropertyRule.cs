using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class ObjectPropertyRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "object-property";

        public string RULE_TEXT => "Expected use of SYS.COLUMNS rather than ObjectProperty function";

        private readonly Action<string, string, int, int> ErrorCallback;

        public ObjectPropertyRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("OBJECTPROPERTY", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }
    }
}
