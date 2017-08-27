using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetAnsiNullsRule : TSqlFragmentVisitor, ISqlRule
    {
        private bool errorLogged;

        public SetAnsiNullsRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "set-ansi"; }
        }

        public string RuleText
        {
            get { return "Expected SET ANSI_NULLS ON near top of file"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(TSqlScript node)
        {
            var childAnsiNullsVisitor = new ChildAnsiNullsVisitor();
            node.AcceptChildren(childAnsiNullsVisitor);
            if (!childAnsiNullsVisitor.SetAnsiNullsFound && !this.errorLogged)
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
                this.errorLogged = true;
            }
        }

        public class ChildAnsiNullsVisitor : TSqlFragmentVisitor
        {
            public bool SetAnsiNullsFound { get; set; }

            public override void Visit(SetOnOffStatement node)
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode != null && typedNode.Options == SetOptions.AnsiNulls)
                {
                    SetAnsiNullsFound = true;
                }
            }
        }
    }
}