using System.Text;
using System.Reflection;
using TSQLLint.Common;

namespace TSQLLint.Lib.Rules.RuleViolations
{
    public class RuleViolation : IRuleViolation
    {
        public int Column { get; set; }

        public string FileName { get; set; }

        public int Line { get; set; }

        public string RuleName { get; set; }

        public RuleViolationSeverity Severity { get; set; }

        public string Text { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

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

        public override string ToString()
        {
            if(_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }
    }
}
