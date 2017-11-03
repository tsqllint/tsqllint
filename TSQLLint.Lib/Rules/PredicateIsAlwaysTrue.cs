using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class PredicateIsAlwaysTrue : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "predicate-is-always-true";

        public string RULE_TEXT => "Found the same on the left and the right side of a Predicate";

        private readonly Action<string, string, int, int> ErrorCallback;

        public PredicateIsAlwaysTrue(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }
        
        public override void Visit(BooleanComparisonExpression node)
        {
            {
                var first = node.FirstExpression as Literal;
                var second = node.SecondExpression as Literal;

                if (first != null && second != null)
                {
                    if (string.Equals(first.Value, second.Value))
                    {
                        ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                    }
                }
            }
            
            {
                var first = node.FirstExpression as ColumnReferenceExpression;
                var second = node.SecondExpression as ColumnReferenceExpression;

                if (first != null && second != null)
                {
                    var firstCol = first.MultiPartIdentifier.Identifiers.Select(x => x.Value);
                    var secondCol = second.MultiPartIdentifier.Identifiers.Select(x => x.Value);

                    if (firstCol.SequenceEqual(secondCol))
                    {
                        ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                    }
                }
            }
        }
    }
}
