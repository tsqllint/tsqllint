using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class FullTextRule : BaseRuleVisitor, ISqlRule
    {
        public FullTextRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "full-text";

        public override string RULE_TEXT => "Full text predicate found, this can cause performance problems";

        public override void Visit(FullTextPredicate node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }
    }
}
