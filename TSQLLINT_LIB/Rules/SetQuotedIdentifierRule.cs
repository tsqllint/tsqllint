using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetQuotedIdentifierRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-quoted"; } }
        public string RULE_TEXT { get { return "SET QUOTED_IDENTIFIER ON at top of file"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool SetNoCountFound;
        private bool ErrorLogged;

        public SetQuotedIdentifierRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(PredicateSetStatement node)
        {
            if (node.Options == SetOptions.QuotedIdentifier)
            {
                SetNoCountFound = true;
            }
        }

        public override void Visit(TSqlStatement node)
        {
            var nodeType = node.GetType();

            if (nodeType == typeof(SetTransactionIsolationLevelStatement))
            {
                return;
            }

            if (nodeType == typeof(PredicateSetStatement))
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode.Options == SetOptions.AnsiNulls || 
                    typedNode.Options == SetOptions.QuotedIdentifier ||
                    typedNode.Options == SetOptions.NoCount)
                {
                    return;
                }
            }

            if (!SetNoCountFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }
    }
}