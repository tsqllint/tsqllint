using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class CrossDatabaseTransactionRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public CrossDatabaseTransactionRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "cross-database-transaction";

        public string RULE_TEXT => "Cross database inserts or updates enclosed in a transaction can lead to data corruption";

        public int DynamicSqlStartColumn { get; set; }

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
                    errorCallback(
                        RULE_NAME,
                        RULE_TEXT,
                        transaction.Begin.StartLine,
                        transaction.Begin.StartColumn + DynamicSqlStartColumn);
                }
            }
        }

        public class TrackedTransaction
        {
            public BeginTransactionStatement Begin { get; set; }

            public CommitTransactionStatement Commit { get; set; }
        }

        public class ChildTransactionVisitor : TSqlFragmentVisitor
        {
            public List<TrackedTransaction> TransactionLists { get; } = new List<TrackedTransaction>();

            public override void Visit(BeginTransactionStatement node)
            {
                TransactionLists.Add(new TrackedTransaction { Begin = node });
            }

            public override void Visit(CommitTransactionStatement node)
            {
                var firstUncomitted = TransactionLists.LastOrDefault(x => x.Commit == null);
                if (firstUncomitted != null)
                {
                    firstUncomitted.Commit = node;
                }
            }
        }

        public class ChildInsertUpdateQueryVisitor : TSqlFragmentVisitor
        {
            private readonly TrackedTransaction transaction;

            private readonly ChildDatabaseNameVisitor childDatabaseNameVisitor = new ChildDatabaseNameVisitor();

            public ChildInsertUpdateQueryVisitor(TrackedTransaction transaction)
            {
                this.transaction = transaction;
            }

            public HashSet<string> DatabasesUpdated { get; } = new HashSet<string>();

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
                if (IsWithinTransaction(node))
                {
                    node.Accept(childDatabaseNameVisitor);
                    DatabasesUpdated.UnionWith(childDatabaseNameVisitor.DatabasesUpdated);
                }
            }

            private bool IsWithinTransaction(TSqlFragment node)
            {
                if (node.StartLine == transaction.Begin?.StartLine &&
                    node.StartColumn < transaction.Begin?.StartColumn)
                {
                    return false;
                }

                if (node.StartLine == transaction.Commit?.StartLine &&
                    node.StartColumn > transaction.Commit?.StartColumn)
                {
                    return false;
                }

                return node.StartLine >= transaction.Begin?.StartLine && node.StartLine <= transaction.Commit?.StartLine;
            }
        }

        public class ChildDatabaseNameVisitor : TSqlFragmentVisitor
        {
            public HashSet<string> DatabasesUpdated { get; } = new HashSet<string>();

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
