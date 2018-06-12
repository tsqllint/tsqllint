using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Rules.RuleViolations
{
    public class RuleViolation : IRuleViolation
    {
        private PropertyInfo[] propertyInfos = null;

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

        public int Column { get; set; }

        public string FileName { get; set; }

        public int Line { get; set; }

        public string RuleName { get; set; }

        public RuleViolationSeverity Severity { get; set; }

        public string Text { get; set; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            if (propertyInfos == null)
            {
                propertyInfos = this.GetType().GetProperties();
            }

            var sb = new StringBuilder();

            foreach (var info in propertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }
    }
}
