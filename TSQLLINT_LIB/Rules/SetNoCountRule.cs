using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetNoCountRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-nocount"; } }
        public string RULE_TEXT { get { return "SET NOCOUNT ON at top of file"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool ErrorLogged;

        public SetNoCountRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childNoCountVisitor = new ChildNoCountVisitor();
            node.AcceptChildren(childNoCountVisitor);
            if (!childNoCountVisitor.SetNoCountFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }

        public class ChildNoCountVisitor : TSqlFragmentVisitor
        {
            public bool SetNoCountFound;

            public override void Visit(SetOnOffStatement node)
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode != null && typedNode.Options == SetOptions.NoCount)
                {
                    SetNoCountFound = true;
                }
            }
        }
    }
}