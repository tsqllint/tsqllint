namespace TSQLLint.Lib.Parser.RuleExceptions
{
    public class GlobalRuleException : IExtendedRuleException
    {
        public GlobalRuleException(int startLine, int endLine)
        {
            EndLine = endLine;
            StartLine = startLine;
        }

        public int EndLine { get; private set; }

        public string RuleName => "Global";

        public int StartLine { get; }

        public void SetEndLine(int endLine)
        {
            EndLine = endLine;
        }
    }
}
