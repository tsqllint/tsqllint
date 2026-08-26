using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class NonSargableRule : BaseRuleVisitor, ISqlRule
    {
        private readonly List<TSqlFragment> errorsReported = new();

        public NonSargableRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "non-sargable";

        public override string RULE_TEXT => "Performing functions on filter clauses or join predicates can cause performance problems";

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
            var multiClauseQuery = predicateExpressionVisitor.PredicatesFound;

            var childVisitor = new FunctionVisitor(VisitorCallback, multiClauseQuery);
            node.Accept(childVisitor);
        }

        private void VisitorCallback(TSqlFragment childNode)
        {
            if (errorsReported.Contains(childNode))
            {
                return;
            }

            var dynamicSqlColumnAdjustment = GetDynamicSqlColumnOffset(childNode);

            errorsReported.Add(childNode);
            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(childNode), ColumnNumberCalculator.GetNodeColumnPosition(childNode) + dynamicSqlColumnAdjustment);
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
                if (node.FunctionName.Value.ToUpper() == "ISNULL" && isMultiClause)
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
                node.Accept(columnReferenceVisitor);

                if (columnReferenceVisitor.ColumnReferenceFound)
                {
                    childCallback(node);
                }
            }
        }

        private class ColumnReferenceVisitor : TSqlFragmentVisitor
        {
            // The first argument of these date functions is a datepart keyword
            // (MONTH, DAY, YEAR, ...) which ScriptDom parses as a
            // ColumnReferenceExpression even though it is not a real column.
            private static readonly HashSet<string> DatePartFunctions = new(StringComparer.OrdinalIgnoreCase)
            {
                "DATEADD",
                "DATEDIFF",
                "DATEDIFF_BIG",
                "DATENAME",
                "DATEPART",
                "DATETRUNC",
                "DATE_BUCKET"
            };

            private readonly HashSet<TSqlFragment> datePartPseudoColumns = new();

            public bool ColumnReferenceFound { get; private set; }

            public override void Visit(FunctionCall node)
            {
                // Exclude the datepart pseudo-column so that nested date functions
                // are not mistaken for genuine column references. (#325)
                if (DatePartFunctions.Contains(node.FunctionName.Value) && node.Parameters.Count > 0)
                {
                    datePartPseudoColumns.Add(node.Parameters[0]);
                }
            }

            public override void Visit(ColumnReferenceExpression node)
            {
                if (datePartPseudoColumns.Contains(node))
                {
                    return;
                }

                ColumnReferenceFound = true;
            }
        }
    }
}
