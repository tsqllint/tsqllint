using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DisallowCursorRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "disallow-cursors";}}
        public string RULE_TEXT { get { return "Use of cursors is not permitted."; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public DisallowCursorRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CursorStatement node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }
    }
}
