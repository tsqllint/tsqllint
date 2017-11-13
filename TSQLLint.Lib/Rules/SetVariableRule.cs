using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class SetVariableRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "set-variable";

        public string RULE_TEXT => "Expected variable to be set using SELECT statement";

        private readonly Action<string, string, int, int> ErrorCallback;

        public SetVariableRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SetVariableStatement node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
