using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetVariableRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-variable"; } }
        public string RULE_TEXT { get { return "Expected variable to be set using SELECT statement"; } }
        public Action<string, string, int, int> ErrorCallback;

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