using System;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Lib.Parser.RuleExceptions
{
    public class RuleException : IRuleException
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

        public void SetEndLine(int endLine)
        {
            EndLine = endLine;
        }
    }
}
