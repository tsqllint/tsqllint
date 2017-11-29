using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class CrossDatabaseTransactionRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "cross-database-transaction";

        public string RULE_TEXT => "Cross database inserts or updates wrapped in a single transaction can lead to data corruption";

        private readonly Action<string, string, int, int> ErrorCallback;

        public CrossDatabaseTransactionRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlBatch node)
        {
            var childTransactionVisitor = new ChildTransactionVisitor();
            node.Accept(childTransactionVisitor);
            foreach (var transaction in childTransactionVisitor.TransactionLists)
            {
                var childInsertUpdateQueryVisitor = new ChildInsertUpdateQueryVisitor(transaction);
                node.Accept(childInsertUpdateQueryVisitor);
                if (childInsertUpdateQueryVisitor.DatabasesUpdated.Count > 1)
                {
                    ErrorCallback(RULE_NAME,
                        RULE_TEXT,
                        transaction.Begin.StartLine,
                        transaction.Begin.StartColumn);
                }
            }
        }

        public class TrackedTransaction
        {
            public BeginTransactionStatement Begin;
            public CommitTransactionStatement Commit;
        }

        public class ChildTransactionVisitor : TSqlFragmentVisitor
        {
            public List<TrackedTransaction> TransactionLists = new List<TrackedTransaction>();

            public override void Visit(BeginTransactionStatement node)
            {
                TransactionLists.Add(new TrackedTransaction { Begin = node } );
            }

            public override void Visit(CommitTransactionStatement node)
            {
                var firstUncomitted = TransactionLists.Last(x => x.Commit == null);
                firstUncomitted.Commit = node;
            }
        }

        public class ChildInsertUpdateQueryVisitor : TSqlFragmentVisitor
        {
            public HashSet<string> DatabasesUpdated = new HashSet<string>();
            
            private readonly TrackedTransaction _transaction;
            private readonly ChildDatabaseNameVisitor childDatabaseNameVisitor = new ChildDatabaseNameVisitor();

            public ChildInsertUpdateQueryVisitor(TrackedTransaction transaction)
            {
                _transaction = transaction;
            }

            public override void Visit(InsertStatement node)
            {
                GetDatabasesUpdated(node);
            }

            public override void Visit(UpdateStatement node)
            {
                GetDatabasesUpdated(node);
            }

            private void GetDatabasesUpdated(TSqlFragment node)
            {
                if (node.StartLine >= _transaction.Begin.StartLine && node.StartColumn > _transaction.Begin.StartColumn)
                {
                    node.Accept(childDatabaseNameVisitor);
                    DatabasesUpdated.UnionWith(childDatabaseNameVisitor.DatabasesUpdated);
                }
            }
        }

        public class ChildDatabaseNameVisitor : TSqlFragmentVisitor
        {
            public HashSet<string> DatabasesUpdated = new HashSet<string>();

            public override void Visit(NamedTableReference node)
            {
                if (node.SchemaObject.DatabaseIdentifier != null)
                {
                    DatabasesUpdated.Add(node.SchemaObject.DatabaseIdentifier.Value);
                }
            }
        }
    }
}
