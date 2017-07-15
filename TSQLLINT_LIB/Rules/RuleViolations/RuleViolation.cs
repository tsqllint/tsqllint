using Microsoft.SqlServer.TransactSql.ScriptDom;

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

        public RuleViolation(string fileName, string ruleName, string text, TSqlFragment fragment, RuleViolationSeverity severity)
        {
            FileName = fileName;
            Column = fragment.StartColumn;
            Line = fragment.StartLine;
            RuleName = ruleName;
            Severity = severity;
            Text = text;
        }

        public RuleViolation(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }
    }
}
