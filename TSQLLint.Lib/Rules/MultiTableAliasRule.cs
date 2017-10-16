using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class MultiTableAliasRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "multi-table-alias";

        public string RULE_TEXT => "Unaliased table found in multi table joins";

        private readonly Action<string, string, int, int> ErrorCallback;

        private HashSet<string> CteNames = new HashSet<string>();

        public MultiTableAliasRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            var childCommonTableExpressionVisitor = new ChildCommonTableExpressionVisitor();
            node.AcceptChildren(childCommonTableExpressionVisitor);
            CteNames = childCommonTableExpressionVisitor.CommonTableExpressionIdentifiers;
        }

        public override void Visit(TableReference node)
        {
            void ChildCallback(TSqlFragment childNode)
            {
                var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(childNode.StartLine, childNode.LastTokenIndex, childNode.ScriptTokenStream);
                var column = ColumnNumberCounter.GetColumnNumberBeforeToken(tabsOnLine, childNode.ScriptTokenStream[childNode.FirstTokenIndex]);
                ErrorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, column);
            }

            var childTableJoinVisitor = new ChildTableJoinVisitor();
            node.AcceptChildren(childTableJoinVisitor);

            if (!childTableJoinVisitor.TableJoined)
            {
                return;
            }

            var childTableAliasVisitor = new ChildTableAliasVisitor(ChildCallback, CteNames);
            node.AcceptChildren(childTableAliasVisitor);
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
            private readonly Action<TSqlFragment> ChildCallback;

            public HashSet<string> CteNames { get; }

            public ChildTableAliasVisitor(Action<TSqlFragment> errorCallback, HashSet<string> cteNames)
            {
                CteNames = cteNames;
                ChildCallback = errorCallback;
            }

            public override void Visit(NamedTableReference node)
            {
                if (CteNames.Contains(node.SchemaObject.BaseIdentifier.Value))
                {
                    return;
                }

                if (node.Alias == null)
                {
                    ChildCallback(node);
                }
            }
        }
    }
}
