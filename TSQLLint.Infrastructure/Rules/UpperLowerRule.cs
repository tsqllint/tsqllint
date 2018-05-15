using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class UpperLowerRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public UpperLowerRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "upper-lower";

        public string RULE_TEXT => "Use of the UPPER or LOWER functions when performing comparisons in SELECT statements is not required when running database in case insensitive mode";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }

        public override void Visit(SelectStatement node)
        {
            var visitor = new ChildQueryComparisonVisitor();
            node.Accept(visitor);
            if (visitor.QueryExpressionUpperLowerFunctionFound)
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }

        public class ChildQueryComparisonVisitor : TSqlFragmentVisitor
        {
            public bool QueryExpressionUpperLowerFunctionFound { get; private set; }

            public override void Visit(QueryExpression node)
            {
                var visitor = new ChildBooleanComparisonVisitor();
                node.Accept(visitor);
                if (visitor.UpperLowerFunctionCallInComparison)
                {
                    QueryExpressionUpperLowerFunctionFound = true;
                }
            }
        }

        public class ChildBooleanComparisonVisitor : TSqlFragmentVisitor
        {
            public bool UpperLowerFunctionCallInComparison { get; private set; }

            public override void Visit(BooleanComparisonExpression node)
            {
                var visitor = new ChildFunctionCallVisitor();
                node.Accept(visitor);
                if (visitor.UpperLowerFound)
                {
                    UpperLowerFunctionCallInComparison = true;
                }
            }
        }

        public class ChildFunctionCallVisitor : TSqlFragmentVisitor
        {
            public bool UpperLowerFound { get; private set; }

            public override void Visit(FunctionCall node)
            {
                if (node.FunctionName.Value.Equals("UPPER", StringComparison.OrdinalIgnoreCase) ||
                    node.FunctionName.Value.Equals("LOWER", StringComparison.OrdinalIgnoreCase))
                {
                    UpperLowerFound = true;
                }
            }
        }
    }
}
