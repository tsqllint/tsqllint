using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "set-quoted-identifier";

        public string RULE_TEXT => "Expected SET QUOTED_IDENTIFIER ON near top of file";

        private readonly Action<string, string, int, int> ErrorCallback;

        private bool ErrorLogged;

        public SetQuotedIdentifierRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);
            
            if (childQuotedidentifierVisitor.QuotedIdentifierFound || ErrorLogged)
            {
                return;
            }
            
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            ErrorLogged = true;
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
