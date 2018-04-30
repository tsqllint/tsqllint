using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private bool errorLogged;

        public SetQuotedIdentifierRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "set-quoted-identifier";

        public string RULE_TEXT => "Expected SET QUOTED_IDENTIFIER ON near top of file";

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);

            if (childQuotedidentifierVisitor.QuotedIdentifierFound || errorLogged)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            errorLogged = true;
        }

        public class ChildQuotedidentifierVisitor : TSqlFragmentVisitor
        {
            public bool QuotedIdentifierFound { get; set; }

            public override void Visit(PredicateSetStatement node)
            {
                if (node.Options == SetOptions.QuotedIdentifier)
                {
                    QuotedIdentifierFound = true;
                }
            }
        }
    }
}
