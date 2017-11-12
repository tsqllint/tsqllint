using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class DisallowCursorRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "disallow-cursors";

        public string RULE_TEXT => "Found use of CURSOR statement";

        private readonly Action<string, string, int, int> ErrorCallback;

        public DisallowCursorRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CursorStatement node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
