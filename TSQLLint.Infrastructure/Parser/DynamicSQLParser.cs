using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Infrastructure.Parser
{
    public class DynamicSQLParser : TSqlFragmentVisitor
    {
        private readonly Action<string> callback;
        private string executableSql = string.Empty;
        private Dictionary<string, string> VariableValues = new Dictionary<string, string>();

        public DynamicSQLParser(Action<string> callback)
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
                switch (executableString)
                {
                    case StringLiteral literal:
                        HandleLiteral(counter, executableStrings.Strings.Count, literal);
                        break;
                    case VariableReference variableReference:
                        HandleVariable(counter, executableStrings.Strings.Count, variableReference);
                        break;
                }
            }
        }

        private void HandleVariable(int counter, int executableCount, VariableReference variableReference)
        {
            if (!VariableValues.ContainsKey(variableReference.Name) || !VariableValues.TryGetValue(variableReference.Name, out var value))
            {
                return;
            }

            executableSql += value;
            if (counter == executableCount)
            {
                callback(executableSql);
            }
        }

        private void HandleLiteral(int counter, int executableCount, StringLiteral literal)
        {
            executableSql += literal.Value;
            if (counter == executableCount)
            {
                callback(executableSql);
            }
        }
    }

    public class VariableVisitor : TSqlFragmentVisitor
    {
        public VariableVisitor()
        {
            VariableValues = new Dictionary<string, string>();
        }

        public Dictionary<string, string> VariableValues { get; }
        
        public override void Visit(SetVariableStatement node)
        {
            if (node.Expression is StringLiteral literal)
            {
                VariableValues.Add(node.Variable.Name, literal.Value);
            }
            else if (node.Expression is BinaryExpression)
            {
                var childVisitor = new BinaryExpressionVisitor();
                node.AcceptChildren(childVisitor);
                VariableValues.Add(childVisitor.Name, childVisitor.Value);
            }
        }

        public class BinaryExpressionVisitor : TSqlFragmentVisitor
        {
            public string Name { get; private set; }
            public string Value { get; private set; }

            public override void Visit(VariableReference node)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = node.Name;
                    return;
                }

                Value = string.Format("{0} = {1}", Value, node.Name);
            }

            public override void Visit(StringLiteral node)
            {
                Value = string.Format("{0} {1}", Value, node.Value);
            }
        }
    }
}
