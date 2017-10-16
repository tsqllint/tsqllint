using TSQLLint.Common;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class TestRuleViolation : IRuleViolation
    {
        public TestRuleViolation(string fileName, string ruleName, string text, int lineNumber, int column, RuleViolationSeverity ruleViolationSeverity)
        {
            FileName = fileName;
            RuleName = ruleName;
            Text = text;
            Line = lineNumber;
            Column = column;
            Severity = ruleViolationSeverity;
        }

        public int Column { get; }

        public string FileName { get; }

        public int Line { get; }

        public string RuleName { get; }

        public RuleViolationSeverity Severity { get; }

        public string Text { get; }
    }
}
