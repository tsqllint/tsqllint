using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class MultiTableAliasRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "multi-table-alias";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Unaliased table found in multi table joins";
            }
        }

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
            Action<TSqlFragment> ChildCallback = delegate(TSqlFragment childNode) 
            {
                var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(childNode.StartLine, childNode.LastTokenIndex, childNode.ScriptTokenStream);
                var column = ColumnNumberCounter.GetColumnNumberBeforeToken(tabsOnLine, childNode.ScriptTokenStream[childNode.FirstTokenIndex]);
                ErrorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, column);
            };

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
            private readonly HashSet<string> _CommonTableExpressionIdentifiers = new HashSet<string>();

            public HashSet<string> CommonTableExpressionIdentifiers
            {
                get
                {
                    return _CommonTableExpressionIdentifiers;
                }
            }

            public override void Visit(CommonTableExpression node)
            {
                _CommonTableExpressionIdentifiers.Add(node.ExpressionName.Value);
            }
        }

        public class ChildTableJoinVisitor : TSqlFragmentVisitor
        {
            private bool _TableJoined;

            public bool TableJoined
            {
                get
                {
                    return _TableJoined;
                }

                private set
                {
                    _TableJoined = value;
                }
            }

            public override void Visit(JoinTableReference node)
            {
                TableJoined = true;
            }
        }

        public class ChildTableAliasVisitor : TSqlFragmentVisitor
        {
            private readonly Action<TSqlFragment> ChildCallback;

            private HashSet<string> _CteNames;

            public HashSet<string> CteNames
            {
                get
                {
                    return _CteNames;
                }

                private set
                {
                    _CteNames = value;
                }
            }

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