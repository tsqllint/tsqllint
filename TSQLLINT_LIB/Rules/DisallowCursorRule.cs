using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DisallowCursorRule : TSqlFragmentVisitor, ISqlRule
    {
        public DisallowCursorRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "disallow-cursors"; }
        }

        public string RuleText
        {
            get { return "Found use of CURSOR statement"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(CursorStatement node)
        {
            ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
        }
    }
}
