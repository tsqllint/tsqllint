using System;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.RuleExceptions
{
    public class RuleException : IExtendedRuleException
    {
        public RuleException(Type ruleType, string ruleName, int startLine, int endLine)
        {
            RuleType = ruleType;
            RuleName = ruleName;
            StartLine = startLine;
            EndLine = endLine;
        }

        public Type RuleType { get; }

        public int StartLine { get; }

        public int EndLine { get; private set; }

        public string RuleName { get; }

        public void SetEndLine(int endLine)
        {
            EndLine = endLine;
        }
    }
}
