using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;

namespace TSQLLint.Infrastructure.Parser
{
    public class DynamicSQLParser : TSqlFragmentVisitor
    {
        private readonly Action<string, int, int> callback;
        private string executableSql = string.Empty;
        private Dictionary<string, VariableVisitor.VariableRef> VariableValues = new ();

        private int DynamicSQLStartingLine { get; set; }
        
        private int DynamicSQLStartingColumn { get; set; }

        public DynamicSQLParser(Action<string, int, int> callback)
        {
            this.callback = callback;
        }

        public override void Visit(TSqlBatch node)
        {
            var variableVisitor = new VariableVisitor();
            node.Accept(variableVisitor);
            VariableValues = variableVisitor.VariableValues;
        }

        public override void Visit(ExecuteStatement node)
        {
            DynamicSQLStartingColumn = node.ExecuteSpecification.ExecutableEntity.StartColumn;
            DynamicSQLStartingLine = node.ExecuteSpecification.ExecutableEntity.StartLine;

            var visitor = new VariableVisitor();
            node.Accept(visitor);

            var executableStrings = node.ExecuteSpecification.ExecutableEntity as ExecutableStringList;
            if (executableStrings?.Strings == null)
            {
                return;
            }

            var counter = 0;
            foreach (var executableString in executableStrings.Strings)
            {
                counter++;
                if (executableString is StringLiteral literal)
                {
                    HandleLiteral(counter, executableStrings.Strings.Count, literal);
                }
                else if (executableString is VariableReference variableReference)
                {
                    HandleVariable(counter, executableStrings.Strings.Count, variableReference);
                }
            }
        }

        private void HandleVariable(int counter, int executableCount, VariableReference variableReference)
        {
            if (!VariableValues.ContainsKey(variableReference.Name) || !VariableValues.TryGetValue(variableReference.Name, out var value))
            {
                return;
            }

            executableSql += value.Value;

            if (counter == executableCount)
            {
                callback(executableSql, value.StartLine, value.StartColumn);
            }
        }

        private void HandleLiteral(int counter, int executableCount, StringLiteral literal)
        {
            executableSql += literal.Value;
            if (counter == executableCount)
            {
                callback(executableSql, DynamicSQLStartingLine, DynamicSQLStartingColumn);
            }
        }
    }

    public class VariableVisitor : TSqlFragmentVisitor
    {
        public Dictionary<string, VariableRef> VariableValues { get; } = new ();

        public override void Visit(SelectSetVariable node)
        {
            HandleExpression(node.Variable.Name, node.Expression);
        }

        public override void Visit(SetVariableStatement node)
        {
            HandleExpression(node.Variable.Name, node.Expression);
        }

        private void HandleExpression(string name, ScalarExpression expression)
        {
            switch (expression)
            {
                case StringLiteral strLiteral:
                    VariableValues[name] = new VariableRef(strLiteral);
                    break;
                case IntegerLiteral intLiteral:
                    VariableValues[name] = new VariableRef(intLiteral);
                    break;
                case BinaryExpression binaryExpression:
                    HandleBinaryExpression(name, binaryExpression);
                    break;
            }
        }

        private void HandleBinaryExpression(string name, BinaryExpression expression)
        {
            if (expression.BinaryExpressionType != BinaryExpressionType.Add)
            {
                return;
            }
            
            if (expression.FirstExpression is StringLiteral first
                && expression.SecondExpression is StringLiteral second)
            {
                VariableValues[name] = new VariableRef(first)
                {
                    Value = first.Value + second.Value
                };
            }
        }

        public struct VariableRef
        {
            public VariableRef(StringLiteral stringLiteral)
                : this((Literal)stringLiteral)
            {
            }

            public VariableRef(IntegerLiteral integerLiteral)
                : this((Literal)integerLiteral)
            {
            }

            private VariableRef(Literal literal)
            {
                StartColumn = literal.StartColumn;
                StartLine = literal.StartLine;
                Value = literal.Value;
            }

            public int StartColumn { get; set; }
            public int StartLine { get; set; }
            public string Value { get; set; }
        }
    }
}
