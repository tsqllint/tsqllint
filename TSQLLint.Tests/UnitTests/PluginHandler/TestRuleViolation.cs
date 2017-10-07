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

        public int Column { get; private set; }

        public string FileName { get; private set; }

        public int Line { get; private set; }

        public string RuleName { get; private set; }

        public RuleViolationSeverity Severity { get; private set; }

        public string Text { get; private set; }
    }
}