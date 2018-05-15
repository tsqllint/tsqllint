using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class NonSargableRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private readonly List<TSqlFragment> errorsReported = new List<TSqlFragment>();

        private bool multiClauseQuery;

        public NonSargableRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "non-sargable";

        public string RULE_TEXT => "Performing functions on filter clauses or join predicates can cause performance problems";

        public int DynamicSqlStartColumn { get; set; }

        public override void Visit(JoinTableReference node)
        {
            var predicateExpressionVisitor = new PredicateVisitor();
            node.AcceptChildren(predicateExpressionVisitor);
            var multiClauseQuery = predicateExpressionVisitor.PredicatesFound;
            
            var joinVisitor = new JoinQueryVisitor(VisitorCallback, multiClauseQuery);
            node.AcceptChildren(joinVisitor);
        }

        public override void Visit(WhereClause node)
        {
            var predicateExpressionVisitor = new PredicateVisitor();
            node.Accept(predicateExpressionVisitor);
            multiClauseQuery = predicateExpressionVisitor.PredicatesFound;

            var childVisitor = new FunctionVisitor(VisitorCallback, multiClauseQuery);
            node.Accept(childVisitor);
        }
        
        private void VisitorCallback(TSqlFragment childNode)
        {
            if (errorsReported.Contains(childNode))
            {
                return;
            }

            errorsReported.Add(childNode);
            errorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, ColumnNumberCalculator.GetNodeColumnPosition(childNode));
        }

        private class JoinQueryVisitor : TSqlFragmentVisitor
        {
            private readonly Action<TSqlFragment> childCallback;
            private readonly bool isMultiClauseQuery;

            public JoinQueryVisitor(Action<TSqlFragment> childCallback, bool multiClauseQuery)
            {
                this.childCallback = childCallback;
                isMultiClauseQuery = multiClauseQuery;
            }

            public override void Visit(BooleanComparisonExpression node)
            {
                var childVisitor = new FunctionVisitor(childCallback, isMultiClauseQuery);
                node.Accept(childVisitor);
            }
        }

        private class PredicateVisitor : TSqlFragmentVisitor
        {
            public bool PredicatesFound { get; private set; }

            public override void Visit(BooleanBinaryExpression node)
            {
                PredicatesFound = true;
            }
        }

        private class FunctionVisitor : TSqlFragmentVisitor
        {
            private readonly bool isMultiClause;
            private readonly Action<TSqlFragment> childCallback;

            public FunctionVisitor(Action<TSqlFragment> errorCallback, bool isMultiClause)
            {
                childCallback = errorCallback;
                this.isMultiClause = isMultiClause;
            }

            public override void Visit(FunctionCall node)
            {
                // allow isnull predicates provided other filters exist
                if (node.FunctionName.Value == "ISNULL" && isMultiClause)
                {
                    return;
                }

                FindColumnReferences(node);
            }

            public override void Visit(LeftFunctionCall node)
            {
                FindColumnReferences(node);
            }

            public override void Visit(RightFunctionCall node)
            {
                FindColumnReferences(node);
            }

            public override void Visit(ConvertCall node)
            {
                FindColumnReferences(node);
            }

            public override void Visit(CastCall node)
            {
                FindColumnReferences(node);
            }

            private void FindColumnReferences(TSqlFragment node)
            {
                var columnReferenceVisitor = new ColumnReferenceVisitor();
                node.AcceptChildren(columnReferenceVisitor);

                if (columnReferenceVisitor.ColumnReferenceFound)
                {
                    childCallback(node);
                }
            }
        }

        private class ColumnReferenceVisitor : TSqlFragmentVisitor
        {
            public bool ColumnReferenceFound { get; private set; }

            public override void Visit(ColumnReferenceExpression node)
            {
                ColumnReferenceFound = true;
            }
        }
    }
}
