using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetAnsiNullsRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "set-ansi";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Expected SET ANSI_NULLS ON near top of file";
            }
        }

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
            if (!childAnsiNullsVisitor.SetAnsiIsOn && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                ErrorLogged = true;
            }
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