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

        public override void Visit(JoinTableReference node)
        {
            var childVisitor = new ChildFunctionCallVisitor(ChildCallback);
            node.Accept(childVisitor);
        }

        public override void Visit(WhereClause node)
        {
            var childVisitor = new ChildFunctionCallVisitor(ChildCallback);
            node.Accept(childVisitor);
        }

        public class ChildFunctionCallVisitor : TSqlFragmentVisitor
        {
            private readonly Action<TSqlFragment> ChildCallback;

            public ChildFunctionCallVisitor(Action<TSqlFragment> errorCallback)
            {
                ChildCallback = errorCallback;
            }

            public override void Visit(FunctionCall node)
            {
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

        public class ChildColumnReferenceVisitor : TSqlConcreteFragmentVisitor
        {
            public bool ColumnReferenceFound { get; private set; }

            public override void Visit(ColumnReferenceExpression node)
            {
                ColumnReferenceFound = true;
            }
        }
    }
}
