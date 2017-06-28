using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interfaces;

namespace TSQLLINT_LIB.Rules
{
    public class SetTransactionIsolationLevelRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-transaction-isolation-level"; } }
        public string RULE_TEXT { get { return "Set Transaction Isolation Level Read Uncommitted Should Appear Before Other Statements"; }}
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool TransactionIsolationLevelStatementVisited;
        private bool ErrorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            // Allow ansi nulls, nocount, and quoted identifier statements to precede isolation level statements
            if (node.GetType().Name == "PredicateSetStatement")
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode.Options == SetOptions.AnsiNulls || 
                    typedNode.Options == SetOptions.QuotedIdentifier ||
                    typedNode.Options == SetOptions.NoCount)
                {
                    return;
                }
            }

            if(!TransactionIsolationLevelStatementVisited && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }

        public override void ExplicitVisit(SetTransactionIsolationLevelStatement node)
        {
            TransactionIsolationLevelStatementVisited = true;
        }
    }
}