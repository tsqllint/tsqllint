using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class ConditionalBeginEndRule : BaseRuleVisitor, ISqlRule
    {
        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "conditional-begin-end";

        public override string RULE_TEXT => "Expected BEGIN and END statement within conditional logic block";

        public override void Visit(IfStatement node)
        {
            Foo(node);

            if (node.ElseStatement != null)
            {
                Foo(node.ElseStatement);
            }
        }

        private void Foo(TSqlFragment node)
        {
            var childBeginEndVisitor = new ChildBeginEndVisitor();
            node.AcceptChildren(childBeginEndVisitor);

            if (childBeginEndVisitor.BeginEndBlockFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        private class ChildBeginEndVisitor : TSqlFragmentVisitor
        {
            public bool BeginEndBlockFound
            {
                get;
                private set;
            }

            public override void Visit(BeginEndBlockStatement node)
            {
                BeginEndBlockFound = true;
            }
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
