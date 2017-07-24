using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-quoted-identifier"; } }
        public string RULE_TEXT { get { return "Expected SET QUOTED_IDENTIFIER ON near top of file"; } }
        public Action<string, string, int, int> ErrorCallback;

        private bool ErrorLogged;

        public SetQuotedIdentifierRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);
            if (!childQuotedidentifierVisitor.QuotedIdentifierFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                ErrorLogged = true;
            }
        }

        public class ChildQuotedidentifierVisitor : TSqlFragmentVisitor
        {
            public bool QuotedIdentifierFound;

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