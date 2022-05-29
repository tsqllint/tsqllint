using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetTransactionIsolationLevelRule : BaseNearTopOfFileRule, ISqlRule
    {
        public SetTransactionIsolationLevelRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-transaction-isolation-level";

        public override string RULE_TEXT => "Expected SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED near top of file";

        public override string Insert => "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";

        public override Func<string, bool> Remove =>
            x => x.StartsWith("SET TRANSACTION ISOLATION LEVEL", StringComparison.CurrentCultureIgnoreCase);

        public override void Visit(TSqlScript node)
        {
            var childTransactionIsolationLevelVisitor = new ChildTransactionIsolationLevelVisitor();
            node.AcceptChildren(childTransactionIsolationLevelVisitor);

            if (childTransactionIsolationLevelVisitor.TransactionIsolationLevelFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
