using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class SetAnsiNullsRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "set-ansi";

        public string RULE_TEXT => "Expected SET ANSI_NULLS ON near top of file";

        private readonly Action<string, string, int, int> ErrorCallback;

        private bool ErrorLogged;

        public SetAnsiNullsRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childAnsiNullsVisitor = new ChildAnsiNullsVisitor();
            node.AcceptChildren(childAnsiNullsVisitor);
            
            if (childAnsiNullsVisitor.SetAnsiIsOn || ErrorLogged)
            {
                return;
            }
            
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            ErrorLogged = true;
        }

        public class ChildAnsiNullsVisitor : TSqlFragmentVisitor
        {
            public bool SetAnsiIsOn { get; set; }

            public override void Visit(PredicateSetStatement node)
            {
                if (node.Options == SetOptions.AnsiNulls)
                {
                    SetAnsiIsOn = node.IsOn;
                }
            }
        }
    }
}
