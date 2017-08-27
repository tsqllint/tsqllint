using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class ConditionalBeginEndRule : TSqlFragmentVisitor, ISqlRule
    {
        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "conditional-begin-end"; }
        }

        public string RuleText
        {
            get { return "Expected BEGIN and END statement within conditional logic block"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(IfStatement node)
        {
            var childBeginEndVisitor = new ChildBeginEndVisitor();
            node.AcceptChildren(childBeginEndVisitor);

            if (childBeginEndVisitor.BeginEndBlockFound)
            {
                return;
            }

            ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
        }

        public class ChildBeginEndVisitor : TSqlFragmentVisitor
        {
            public bool BeginEndBlockFound { get; set; }

            public override void Visit(BeginEndBlockStatement node)
            {
                BeginEndBlockFound = true;
            }
        }
    }
}
