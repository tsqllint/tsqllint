using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class FunctionInPredicate : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "function-in-predicate";

        public string RULE_TEXT => "Don’t use functions in the WHERE or JOIN clause, they reduce performance.(index cannot be used)";

        private readonly Action<string, string, int, int> ErrorCallback;

        public FunctionInPredicate(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }
        
        private bool isInScope = false;

        public override void Visit(WhereClause node)
        {
            isInScope = true;
        }

        public override void Visit(FromClause node)
        {
            isInScope = true;
        }

        public override void Visit(SelectStatement node)
        {
            isInScope = false;
        }

        public override void Visit(SetClause node)
        {
            isInScope = false;
        }

        public override void Visit(CheckConstraintDefinition node)
        {
            isInScope = false;
        }

        public override void Visit(BooleanComparisonExpression node)
        {
            if (isInScope)
            {
                var first = node.FirstExpression as FunctionCall;
                var second = node.SecondExpression as FunctionCall;

                if (first != null || second != null)
                {
                    ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                }
            }
        }
    }
}
