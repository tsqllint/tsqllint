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

        /// <param name="fileName"></param>
        /// <param name="ruleName"></param>
        /// <param name="text"></param>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="severity"></param>
        public RuleViolation(string fileName, string ruleName, string text, int startLine, int startColumn, RuleViolationSeverity severity)
        {
            FileName = fileName;
            RuleName = ruleName;
            Text = text;
            Line = startLine;
            Column = startColumn;
            Severity = severity;
        }

        /// <param name="ruleName"></param>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        public RuleViolation(string ruleName, int startLine, int startColumn)
        {
            RuleName = ruleName;
            Line = startLine;
            Column = startColumn;
        }

        /// <param name="fileName"></param>
        /// <param name="text"></param>
        public RuleViolation(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }
    }
}