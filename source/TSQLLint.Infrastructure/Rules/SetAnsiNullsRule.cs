using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetAnsiNullsRule : BaseRuleVisitor, ISqlRule
    {
        private bool errorLogged;

        public SetAnsiNullsRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-ansi";

        public override string RULE_TEXT => "Expected SET ANSI_NULLS ON near top of file";

        public override void Visit(TSqlScript node)
        {
            var childAnsiNullsVisitor = new ChildAnsiNullsVisitor();
            node.AcceptChildren(childAnsiNullsVisitor);

            if (childAnsiNullsVisitor.SetAnsiIsOn || errorLogged)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            errorLogged = true;
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
