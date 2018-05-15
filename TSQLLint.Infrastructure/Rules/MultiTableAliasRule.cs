using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class MultiTableAliasRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private HashSet<string> cteNames = new HashSet<string>();

        public MultiTableAliasRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "multi-table-alias";

        public string RULE_TEXT => "Unaliased table found in multi table joins";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }

        public override void Visit(TSqlStatement node)
        {
            var childCommonTableExpressionVisitor = new ChildCommonTableExpressionVisitor();
            node.AcceptChildren(childCommonTableExpressionVisitor);
            cteNames = childCommonTableExpressionVisitor.CommonTableExpressionIdentifiers;
        }

        public override void Visit(TableReference node)
        {
            void ChildCallback(TSqlFragment childNode)
            {
                var dynamicSqlAdjustment = AdjustColumnForDymamicSQL(childNode);
                var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(childNode.StartLine, childNode.LastTokenIndex, childNode.ScriptTokenStream);
                var column = ColumnNumberCalculator.GetColumnNumberBeforeToken(tabsOnLine, childNode.ScriptTokenStream[childNode.FirstTokenIndex]);
                errorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, column + dynamicSqlAdjustment);
            }

            var childTableJoinVisitor = new ChildTableJoinVisitor();
            node.AcceptChildren(childTableJoinVisitor);

            if (!childTableJoinVisitor.TableJoined)
            {
                return;
            }

            var childTableAliasVisitor = new ChildTableAliasVisitor(ChildCallback, cteNames);
            node.AcceptChildren(childTableAliasVisitor);
        }

        private int AdjustColumnForDymamicSQL(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? DynamicSqlStartColumn
                : 0;
        }

        public class ChildCommonTableExpressionVisitor : TSqlFragmentVisitor
        {
            public HashSet<string> CommonTableExpressionIdentifiers { get; } = new HashSet<string>();

            public override void Visit(CommonTableExpression node)
            {
                CommonTableExpressionIdentifiers.Add(node.ExpressionName.Value);
            }
        }

        public class ChildTableJoinVisitor : TSqlFragmentVisitor
        {
            public bool TableJoined { get; private set; }

            public override void Visit(JoinTableReference node)
            {
                TableJoined = true;
            }
        }

        public class ChildTableAliasVisitor : TSqlFragmentVisitor
        {
            private readonly Action<TSqlFragment> childCallback;

            public ChildTableAliasVisitor(Action<TSqlFragment> errorCallback, HashSet<string> cteNames)
            {
                CteNames = cteNames;
                childCallback = errorCallback;
            }

            public HashSet<string> CteNames { get; }

            public override void Visit(NamedTableReference node)
            {
                if (CteNames.Contains(node.SchemaObject.BaseIdentifier.Value))
                {
                    return;
                }

                if (node.Alias == null)
                {
                    childCallback(node);
                }
            }
        }
    }
}
