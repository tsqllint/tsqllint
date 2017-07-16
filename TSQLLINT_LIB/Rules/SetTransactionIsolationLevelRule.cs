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

        private bool ErrorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childTransactionIsolationLevelVisitor = new ChildTransactionIsolationLevelVisitor();
            node.AcceptChildren(childTransactionIsolationLevelVisitor);
            if (!childTransactionIsolationLevelVisitor.TransactionIsolationLevelFound && !ErrorLogged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
                ErrorLogged = true;
            }
        }

        public class ChildTransactionIsolationLevelVisitor : TSqlFragmentVisitor
        {
            public bool TransactionIsolationLevelFound;

            public override void Visit(SetTransactionIsolationLevelStatement node)
            {
                if (node.Level == IsolationLevel.ReadUncommitted)
                {
                    TransactionIsolationLevelFound = true;
                }
            }
        }
    }
}