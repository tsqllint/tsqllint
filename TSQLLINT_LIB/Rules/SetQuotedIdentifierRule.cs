using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        private bool _errorLogged;

        public SetQuotedIdentifierRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "set-quoted-identifier"; }
        }

        public string RuleText
        {
            get { return "Expected SET QUOTED_IDENTIFIER ON near top of file"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);
            if (!childQuotedidentifierVisitor.QuotedIdentifierFound && !_errorLogged)
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
                _errorLogged = true;
            }
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