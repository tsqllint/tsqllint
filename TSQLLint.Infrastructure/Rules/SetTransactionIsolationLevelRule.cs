using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetTransactionIsolationLevelRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private bool errorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "set-transaction-isolation-level";

        public string RULE_TEXT => "Expected SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED near top of file";

        public int DynamicSqlStartColumn { get; set; }

        public override void Visit(TSqlScript node)
        {
            var childTransactionIsolationLevelVisitor = new ChildTransactionIsolationLevelVisitor();
            node.AcceptChildren(childTransactionIsolationLevelVisitor);

            if (childTransactionIsolationLevelVisitor.TransactionIsolationLevelFound || errorLogged)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            errorLogged = true;
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
