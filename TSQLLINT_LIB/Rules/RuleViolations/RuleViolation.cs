namespace TSQLLINT_LIB.Rules.RuleViolations
{
    public class RuleViolation
    {
        public int Column { get; private set; }
        public string FileName { get; private set; }
        public int Line { get; private set; }
        public string RuleName { get; private set; }
        public RuleViolationSeverity Severity { get; private set; }
        public string Text { get; private set; }

        public RuleViolation(string fileName, string ruleName, string text, int startLine, int startColumn, RuleViolationSeverity severity)
        {
            FileName = fileName;
            RuleName = ruleName;
            Text = text;
            Line = startLine;
            Column = startColumn;
            Severity = severity;
        }

        public RuleViolation(string ruleName, int startLine, int startColumn)
        {
            RuleName = ruleName;
            Line = startLine;
            Column = startColumn;
        }

        public RuleViolation(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }
    }
}