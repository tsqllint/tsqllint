using System;
using TSQLLint.Lib.Standard.Parser.Interfaces;

namespace TSQLLint.Lib.Standard.Parser.RuleExceptions
{
    public class RuleException : IRuleException
    {
        public Type RuleType { get; }
        
        public int StartLine { get; }
        
        public int EndLine { get; private set; }

        public RuleException(Type ruleType, int startLine, int endLine)
        {
            RuleType = ruleType;
            StartLine = startLine;
            EndLine = endLine;
        }

        public void SetEndLine(int endLine)
        {
            EndLine = endLine;
        }
    }
}
