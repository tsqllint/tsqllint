using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetVariableRule : TSqlFragmentVisitor, ISqlRule
    {
        public SetVariableRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "set-variable"; }
        }

        public string RuleText
        {
            get { return "Expected variable to be set using SELECT statement"; }
        }
        
        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(SetVariableStatement node)
        {
            ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
        }
    }
}