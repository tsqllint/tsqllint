using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class ConditionalBeginEndRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "conditional-begin-end";

        public string RULE_TEXT => "Expected BEGIN and END statement within conditional logic block";

        private readonly Action<string, string, int, int> ErrorCallback;

        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(IfStatement node)
        {
            var childBeginEndVisitor = new ChildBeginEndVisitor();
            node.AcceptChildren(childBeginEndVisitor);

            if (childBeginEndVisitor.BeginEndBlockFound)
            {
                return;
            }

            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
