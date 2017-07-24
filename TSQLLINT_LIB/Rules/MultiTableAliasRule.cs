using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Common;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class MultiTableAliasRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "multi-table-alias"; } }
        public string RULE_TEXT { get { return "Unaliased table found in multi table joins"; } }
        public Action<string, string, int, int> ErrorCallback;

        public MultiTableAliasRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TableReference node)
        {
            Action<TSqlFragment> ChildCallback = delegate (TSqlFragment childNode) {

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

            var childTableAliasVisitor = new ChildTableAliasVisitor(ChildCallback);
            node.AcceptChildren(childTableAliasVisitor);
        }

        public class ChildTableJoinVisitor : TSqlFragmentVisitor
        {
            public bool TableJoined;

            public override void Visit(JoinTableReference node)
            {
                TableJoined = true;
            }
        }

        public class ChildTableAliasVisitor : TSqlFragmentVisitor
        {
            public Action<TSqlFragment> ChildCallback;

            public ChildTableAliasVisitor(Action<TSqlFragment> errorCallback)
            {
                ChildCallback = errorCallback;
            }

            public override void Visit(NamedTableReference node)
            {
                if (node.Alias == null)
                {
                    ChildCallback(node);
                }
            }
        }
    }
}