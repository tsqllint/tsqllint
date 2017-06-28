using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interfaces;

namespace TSQLLINT_LIB.Rules
{
    public class SetNoCountRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-nocount"; } }
        public string RULE_TEXT { get { return "SET NOCOUNT ON at top of file"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool SetNoCount;
        private bool ErrorLogged;

        public SetNoCountRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SetOnOffStatement node)
        {
            var typedNode = node as PredicateSetStatement;
            if (typedNode.Options == SetOptions.NoCount)
            {
                SetNoCount = true;
            }
        }

        public override void Visit(TSqlStatement node)
        {
            // Enforce SET NOCOUNT ON before statements
            if (node.GetType().Name == "PredicateSetStatement")
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode.Options == SetOptions.AnsiNulls || typedNode.Options == SetOptions.QuotedIdentifier)
                {
                    return;
                }
            }

            if (!SetNoCount && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }
    }
}