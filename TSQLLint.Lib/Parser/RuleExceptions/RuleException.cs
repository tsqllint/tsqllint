using System;

namespace TSQLLint.Lib.Parser.RuleExceptions
{
    public class RuleException : IExtendedRuleException
    {
        public RuleException(Type ruleType, int startLine, int endLine)
        {
            RuleType = ruleType;
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
