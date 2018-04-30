using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class DisallowCursorRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public DisallowCursorRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "disallow-cursors";

        public string RULE_TEXT => "Found use of CURSOR statement";

        public override void Visit(CursorStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
