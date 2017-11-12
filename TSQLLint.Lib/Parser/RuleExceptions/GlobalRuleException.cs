using TSQLLint.Lib.Standard.Parser.Interfaces;

namespace TSQLLint.Lib.Standard.Parser.RuleExceptions
{
    public class GlobalRuleException : IRuleException
    {
        public int EndLine { get; private set; }
        
        public int StartLine { get; }

        public void SetEndLine(int endLine)
        {
            EndLine = endLine;
        }

        public GlobalRuleException(int startLine, int endLine)
        {
            EndLine = endLine;
            StartLine = startLine;
        }
    }
}
