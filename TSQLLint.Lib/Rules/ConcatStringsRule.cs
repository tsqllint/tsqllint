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
        private readonly Dictionary<string, bool> _stringVariables = new Dictionary<string, bool>();
        private readonly List<ErrorInfo> _errorNodes = new List<ErrorInfo>();

        public ConcatStringsRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        public override void Visit(DeclareVariableElement node)
        {
            var sqlDataType = node.DataType.Name.BaseIdentifier.Value;
            var isNational = sqlDataType.Equals("nvarchar", StringComparison.OrdinalIgnoreCase);
            if (!isNational && !sqlDataType.Equals("varchar", StringComparison.OrdinalIgnoreCase)) return;

            var name = node.VariableName.Value.ToLower();
            if (!_stringVariables.ContainsKey(name))
            {
                _stringVariables.Add(name, isNational);
            }
            else
            {
                _stringVariables[name] = isNational;
            }
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
            bool? IsNationalStringVariable(VariableReference variableReference)
            {
                var name = variableReference.Name.ToLower();
                if (_stringVariables.ContainsKey(name))
                {
                    return _stringVariables[name];
                }
                return null;
            }

            var childExpressionVisitor = new ChildExpressionVisitor(this, IsNationalStringVariable);
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

        private bool IsTypeToCheck(TSqlFragment fragment)
        {
            return fragment is StringLiteral || fragment is BinaryExpression || fragment is VariableReference varRef && _stringVariables.ContainsKey(varRef.Name.ToLower());
        }

        private bool IsExpressionToCheck(BinaryExpression node)
        {
            return node.BinaryExpressionType == BinaryExpressionType.Add && IsTypeToCheck(node.FirstExpression) && IsTypeToCheck(node.SecondExpression);
        }

        private bool IsExpressionToCheck(BooleanComparisonExpression node)
        {
            return IsTypeToCheck(node.FirstExpression) && IsTypeToCheck(node.SecondExpression);
        }

        private class ChildExpressionVisitor : TSqlFragmentVisitor
        {
            private readonly ConcatStringsRule _parent;
            private readonly Func<VariableReference, bool?> _isNationalStringVariable;

            public ChildExpressionVisitor(ConcatStringsRule parent, Func<VariableReference, bool?> isNationalStringVariable)
            {
                _parent = parent;
                _isNationalStringVariable = isNationalStringVariable;
            }

            public readonly List<NodeInfo> Children = new List<NodeInfo>();

            public override void Visit(BinaryExpression node)
            {
                if (!_parent.IsExpressionToCheck(node)) return;

                ProcessChildren(node.FirstExpression);
                ProcessChildren(node.SecondExpression);
            }

            public override void Visit(VariableReference node)
            {
                var isNationalStringVariable = _isNationalStringVariable.Invoke(node);
                if (isNationalStringVariable != null)
                {
                    Children.Add(new NodeInfo { IsNational = isNationalStringVariable.Value });
                }
            }

            public override void Visit(StringLiteral node)
            {
                Children.Add(new NodeInfo { IsNational = node.IsNational });
            }

            private void ProcessChildren(TSqlFragment expression)
            {
                var childExpressionVisitorVisitor = new ChildExpressionVisitor(_parent, _isNationalStringVariable);
                expression.AcceptChildren(childExpressionVisitorVisitor);
                Children.AddRange(childExpressionVisitorVisitor.Children);
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
