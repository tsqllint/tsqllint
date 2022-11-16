using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DisallowCursorRule : BaseRuleVisitor, ISqlRule
    {
        public DisallowCursorRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "disallow-cursors";

        public override string RULE_TEXT => "Found use of CURSOR statement";

        public override void Visit(CursorStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }
    }
}
