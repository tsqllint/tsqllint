using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetQuotedIdentifierRule : BaseNearTopOfFileRule, ISqlRule
    {
        public SetQuotedIdentifierRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-quoted-identifier";

        public override string RULE_TEXT => "Expected SET QUOTED_IDENTIFIER ON near top of file";

        public override string Insert => "SET QUOTED_IDENTIFIER ON;";

        public override Func<string, bool> Remove =>
            x => x.StartsWith("SET QUOTED_IDENTIFIER", StringComparison.CurrentCultureIgnoreCase);

        public override void Visit(TSqlScript node)
        {
            var childQuotedidentifierVisitor = new ChildQuotedidentifierVisitor();
            node.AcceptChildren(childQuotedidentifierVisitor);

            if (childQuotedidentifierVisitor.QuotedIdentifierFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
