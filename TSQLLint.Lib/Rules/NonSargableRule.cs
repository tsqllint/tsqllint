using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class NonSargableRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "non-sargable";

        public string RULE_TEXT => "Performing functions on filter clauses or join predicates can cause performance problems";

        private readonly Action<string, string, int, int> ErrorCallback;

        private readonly List<TSqlFragment> ErrorsReported = new List<TSqlFragment>();

        private bool _multiClauseQuery;

        public NonSargableRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        private void ChildCallback(TSqlFragment childNode)
        {
            if (ErrorsReported.Contains(childNode))
            {
                return;
            }
            
            ErrorsReported.Add(childNode);
            ErrorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, ColumnNumberCalculator.GetNodeColumnPosition(childNode));
        }

        public override void Visit(TSqlStatement node)
        {
            _multiClauseQuery = false;
        }

        public override void Visit(JoinTableReference node)
        {
            var childBinaryExpressionVisitor = new ChildPredicateExpressionVisitor();
            node.Accept(childBinaryExpressionVisitor);
            _multiClauseQuery = childBinaryExpressionVisitor.PredicatesFound;
            
            var childVisitor = new ChildFunctionCallVisitor(ChildCallback, _multiClauseQuery);
            node.Accept(childVisitor);
        }

        public override void Visit(WhereClause node)
        {
            var childBinaryExpressionVisitor = new ChildPredicateExpressionVisitor();
            node.Accept(childBinaryExpressionVisitor);
            _multiClauseQuery = childBinaryExpressionVisitor.PredicatesFound;

            var childVisitor = new ChildFunctionCallVisitor(ChildCallback, _multiClauseQuery);
            node.Accept(childVisitor);
        }

        public class ChildPredicateExpressionVisitor : TSqlFragmentVisitor
        {
            public bool PredicatesFound { get; private set; }

            public override void Visit(BooleanBinaryExpression node)
            {
                PredicatesFound = true;
            }
        }

        public class ChildFunctionCallVisitor : TSqlFragmentVisitor
        {
            private readonly bool _isMultiClause;
            private readonly Action<TSqlFragment> ChildCallback;

            public ChildFunctionCallVisitor(Action<TSqlFragment> errorCallback, bool isMultiClause)
            {
                ChildCallback = errorCallback;
                _isMultiClause = isMultiClause;
            }

            public override void Visit(FunctionCall node)
            {
                // allow isnull predicates provided other filters exist
                if (node.FunctionName.Value == "ISNULL" && _isMultiClause)
                {
                    return;
                }

                VisitChildren(node);
            }

            public override void Visit(LeftFunctionCall node)
            {
                VisitChildren(node);
            }

            public override void Visit(RightFunctionCall node)
            {
                VisitChildren(node);
            }

            public override void Visit(ConvertCall node)
            {
                VisitChildren(node);
            }

            public override void Visit(CastCall node)
            {
                VisitChildren(node);
            }

            private void VisitChildren(TSqlFragment node)
            {
                var columnReferenceVisitor = new ChildColumnReferenceVisitor();
                node.AcceptChildren(columnReferenceVisitor);

                if (columnReferenceVisitor.ColumnReferenceFound)
                {
                    ChildCallback(node);
                }
            }
        }

        public class ChildColumnReferenceVisitor : TSqlFragmentVisitor
        {
            public bool ColumnReferenceFound { get; private set; }

            public override void Visit(ColumnReferenceExpression node)
            {
                ColumnReferenceFound = true;
            }
        }
    }
}
