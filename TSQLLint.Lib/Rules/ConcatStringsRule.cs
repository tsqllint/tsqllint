using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class ConcatStringsRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "concat-strings";

        public string RULE_TEXT => "Strings should be concatenated with like types: varchar to varchar or nvarchar to nvarchar";

        private readonly Action<string, string, int, int> _errorCallback;
        private readonly List<ErrorInfo> _errorNodes = new List<ErrorInfo>();

        public ConcatStringsRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        public override void Visit(BinaryExpression node)
        {
            if (!IsExpressionToCheck(node)) return;

            ProcessChildren(node);
        }

        public override void Visit(BooleanComparisonExpression node)
        {
            if (!IsExpressionToCheck(node)) return;

            ProcessChildren(node);
        }

        private void ProcessChildren(TSqlFragment node)
        {
            var childExpressionVisitor = new ChildExpressionVisitor();
            node.AcceptChildren(childExpressionVisitor);

            if (childExpressionVisitor.Children.Count > 0 &&
                (childExpressionVisitor.Children.TrueForAll(n => n.IsNational) || childExpressionVisitor.Children.TrueForAll(n => !n.IsNational)))
            {
                return;
            }

            AddError(node);
        }

        private void AddError(TSqlFragment fragment)
        {
            var errorInfo = new ErrorInfo(fragment);
            if (_errorNodes.Contains(errorInfo)) return;

            _errorCallback(RULE_NAME, RULE_TEXT, fragment.StartLine, fragment.StartColumn);
            _errorNodes.Add(errorInfo);
        }

        private static bool IsExpressionToCheck(BinaryExpression node)
        {
            return node.BinaryExpressionType == BinaryExpressionType.Add;
        }

        private static bool IsExpressionToCheck(BooleanComparisonExpression node)
        {
            return !(node.FirstExpression is ColumnReferenceExpression) || !(node.SecondExpression is ColumnReferenceExpression);
        }

        private class ChildExpressionVisitor : TSqlFragmentVisitor
        {
            public readonly List<NodeInfo> Children = new List<NodeInfo>();

            public override void Visit(BooleanComparisonExpression node)
            {
                if (!IsExpressionToCheck(node)) return;

                var firstExpressionVisitor = new ChildExpressionVisitor();
                node.FirstExpression.AcceptChildren(firstExpressionVisitor);
                Children.AddRange(firstExpressionVisitor.Children);

                var secondExpressionVisitor = new ChildExpressionVisitor();
                node.SecondExpression.AcceptChildren(secondExpressionVisitor);
                Children.AddRange(secondExpressionVisitor.Children);
            }

            public override void Visit(BinaryExpression node)
            {
                if (!IsExpressionToCheck(node)) return;

                var firstExpressionVisitor = new ChildExpressionVisitor();
                node.FirstExpression.AcceptChildren(firstExpressionVisitor);
                Children.AddRange(firstExpressionVisitor.Children);

                var secondExpressionVisitor = new ChildExpressionVisitor();
                node.SecondExpression.AcceptChildren(secondExpressionVisitor);
                Children.AddRange(secondExpressionVisitor.Children);
            }

            public override void Visit(StringLiteral node)
            {
                Children.Add(new NodeInfo { IsNational = node.IsNational });
            }
        }

        private class NodeInfo
        {
            public bool IsNational { get; set; }
        }

        private class ErrorInfo : IEquatable<ErrorInfo>
        {
            private readonly int _startLine;
            private readonly int _startColumn;

            public ErrorInfo(TSqlFragment node)
            {
                _startLine = node.StartLine;
                _startColumn = node.StartColumn;
            }           

            public bool Equals(ErrorInfo other)
            {
                return other != null && other._startLine == _startLine && other._startColumn == _startColumn;
            }
        }
    }
}
