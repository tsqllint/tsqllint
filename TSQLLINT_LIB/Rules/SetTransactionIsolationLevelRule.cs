using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetTransactionIsolationLevelRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "set-transaction-isolation-level"; } }
        public string RULE_TEXT { get { return "Set Transaction Isolation Level Read Uncommitted Should Appear Before Other Statements"; }}
        public Action<string, string, TSqlFragment> ErrorCallback;

        private bool TransactionIsolationLevelStatementFound;
        private bool ErrorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            var nodeType = node.GetType();

            // Allow ansi nulls, nocount, and quoted identifier statements, 
            // as well as other predicates to precede isolation level statements
            if (nodeType == typeof(PredicateSetStatement))
            {
                return;
            }

            if(!TransactionIsolationLevelStatementFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }

        public override void ExplicitVisit(SetTransactionIsolationLevelStatement node)
        {
            if (node.Level == IsolationLevel.ReadUncommitted)
            {
                TransactionIsolationLevelStatementFound = true;
            }
        }
    }
}