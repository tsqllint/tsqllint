using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Common;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class MultiTableAliasRule : TSqlFragmentVisitor, ISqlRule
    {
        public MultiTableAliasRule(Action<string, string, int, int> errorCallback)
        {
            CteNames = new HashSet<string>();
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "multi-table-alias"; }
        }

        public string RuleText
        {
            get { return "Unaliased table found in multi table joins"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public HashSet<string> CteNames { get; set; }

        public override void Visit(TSqlStatement node)
        {
            var childCommonTableExpressionVisitor = new ChildCommonTableExpressionVisitor();
            node.AcceptChildren(childCommonTableExpressionVisitor);
            CteNames = childCommonTableExpressionVisitor.CommonTableExpressionIdentifiers;
        }

        public override void Visit(TableReference node)
        {
            Action<TSqlFragment> childCallback = childNode => 
            {
                var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(childNode.StartLine, childNode.LastTokenIndex, childNode.ScriptTokenStream);
                var column = ColumnNumberCounter.GetColumnNumberBeforeToken(tabsOnLine, childNode.ScriptTokenStream[childNode.FirstTokenIndex]);
                ErrorCallback(RuleName, RuleText, childNode.StartLine, column);
            };

            var childTableJoinVisitor = new ChildTableJoinVisitor();
            node.AcceptChildren(childTableJoinVisitor);

            if (!childTableJoinVisitor.TableJoined)
            {
                return;
            }

            var childTableAliasVisitor = new ChildTableAliasVisitor(childCallback, CteNames);
            node.AcceptChildren(childTableAliasVisitor);
        }

        public class ChildCommonTableExpressionVisitor : TSqlFragmentVisitor
        {
            public ChildCommonTableExpressionVisitor()
            {
                CommonTableExpressionIdentifiers = new HashSet<string>();
            }

            public HashSet<string> CommonTableExpressionIdentifiers { get; set; }

            public override void Visit(CommonTableExpression node)
            {
                CommonTableExpressionIdentifiers.Add(node.ExpressionName.Value);
            }
        }

        public class ChildTableJoinVisitor : TSqlFragmentVisitor
        {
            public bool TableJoined { get; set; }

            public override void Visit(JoinTableReference node)
            {
                TableJoined = true;
            }
        }

        public class ChildTableAliasVisitor : TSqlFragmentVisitor
        {
            public ChildTableAliasVisitor(Action<TSqlFragment> errorCallback, HashSet<string> cteNames)
            {
                CteNames = cteNames;
                ChildCallback = errorCallback;
            }

            public Action<TSqlFragment> ChildCallback { get; set; }
            public HashSet<string> CteNames { get; set; }

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