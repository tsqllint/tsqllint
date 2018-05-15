using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class ConditionalBeginEndRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "conditional-begin-end";

        public string RULE_TEXT => "Expected BEGIN and END statement within conditional logic block";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }

        public override void Visit(IfStatement node)
        {
            var childBeginEndVisitor = new ChildBeginEndVisitor();
            node.AcceptChildren(childBeginEndVisitor);

            if (childBeginEndVisitor.BeginEndBlockFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn + DynamicSqlStartColumn);
        }

        public class ChildBeginEndVisitor : TSqlFragmentVisitor
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
    }
}
