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

        public override void Visit(TSqlStatement node)
        {
            multiClauseQuery = false;
        }

        public override void Visit(JoinTableReference node)
        {
            var predicateExpressionVisitor = new PredicateExpressionVisitor();
            node.Accept(predicateExpressionVisitor);
            multiClauseQuery = predicateExpressionVisitor.PredicatesFound;

            var childVisitor = new FunctionCallVisitor(ChildCallback, multiClauseQuery);
            node.Accept(childVisitor);
        }

        public override void Visit(WhereClause node)
        {
            var predicateExpressionVisitor = new PredicateExpressionVisitor();
            node.Accept(predicateExpressionVisitor);
            multiClauseQuery = predicateExpressionVisitor.PredicatesFound;

            var childVisitor = new FunctionCallVisitor(ChildCallback, multiClauseQuery);
            node.Accept(childVisitor);
        }

        private void ChildCallback(TSqlFragment childNode)
        {
            if (errorsReported.Contains(childNode))
            {
                return;
            }

            errorsReported.Add(childNode);
            errorCallback(RULE_NAME, RULE_TEXT, childNode.StartLine, ColumnNumberCalculator.GetNodeColumnPosition(childNode));
        }

        public class PredicateExpressionVisitor : TSqlFragmentVisitor
        {
            public bool PredicatesFound { get; private set; }

            public override void Visit(BooleanBinaryExpression node)
            {
                PredicatesFound = true;
            }
        }

        public class FunctionCallVisitor : TSqlFragmentVisitor
        {
            private readonly bool isMultiClause;
            private readonly Action<TSqlFragment> childCallback;

            public FunctionCallVisitor(Action<TSqlFragment> errorCallback, bool isMultiClause)
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

        public class ColumnReferenceVisitor : TSqlFragmentVisitor
        {
            public bool ColumnReferenceFound { get; private set; }

            public override void Visit(ColumnReferenceExpression node)
            {
                ColumnReferenceFound = true;
            }
        }
    }
}
