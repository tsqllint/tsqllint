using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DisallowCursorRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "disallow-cursors";}}
        public string RULE_TEXT { get { return "Found use of CURSOR statement"; } }
        public Action<string, string, int, int> ErrorCallback;

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
