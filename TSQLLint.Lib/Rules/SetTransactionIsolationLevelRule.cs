using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class SetTransactionIsolationLevelRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "set-transaction-isolation-level";

        public string RULE_TEXT => "Expected SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED near top of file";

        private readonly Action<string, string, int, int> ErrorCallback;

        private bool ErrorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            var childTransactionIsolationLevelVisitor = new ChildTransactionIsolationLevelVisitor();
            node.AcceptChildren(childTransactionIsolationLevelVisitor);
            
            if (childTransactionIsolationLevelVisitor.TransactionIsolationLevelFound || ErrorLogged)
            {
                return;
            }
            
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            ErrorLogged = true;
        }

        public class ChildTransactionIsolationLevelVisitor : TSqlFragmentVisitor
        {
            public bool TransactionIsolationLevelFound { get; private set; }

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
