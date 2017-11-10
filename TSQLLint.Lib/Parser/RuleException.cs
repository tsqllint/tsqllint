using System;

namespace TSQLLint.Lib.Parser
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
