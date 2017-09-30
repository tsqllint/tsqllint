using TSQLLINT_COMMON;

namespace TSQLLINT_LIB.Rules.RuleViolations
{
    public class RuleViolation : IRuleViolation
    {
        public int Column { get; set; }

        public string FileName { get; set; }

        public int Line { get; set; }

        public string RuleName { get; set; }

        public RuleViolationSeverity Severity { get; set; }

        public string Text { get; set; }

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
    }
}