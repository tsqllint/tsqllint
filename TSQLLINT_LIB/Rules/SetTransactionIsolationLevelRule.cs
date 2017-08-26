using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetTransactionIsolationLevelRule : TSqlFragmentVisitor, ISqlRule
    {
        private bool _errorLogged;

        public SetTransactionIsolationLevelRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "set-transaction-isolation-level"; }
        }

        public string RuleText
        {
            get { return "Expected SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED near top of file"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(TSqlScript node)
        {
            var childTransactionIsolationLevelVisitor = new ChildTransactionIsolationLevelVisitor();
            node.AcceptChildren(childTransactionIsolationLevelVisitor);
            if (!childTransactionIsolationLevelVisitor.TransactionIsolationLevelFound && !_errorLogged)
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
                _errorLogged = true;
            }
        }

        public class ChildTransactionIsolationLevelVisitor : TSqlFragmentVisitor
        {
            public bool TransactionIsolationLevelFound { get; set; }

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