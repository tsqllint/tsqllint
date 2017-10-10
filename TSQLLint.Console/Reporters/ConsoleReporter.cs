using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;

namespace TSQLLint.Console.Reporters
{
    public class ConsoleReporter : IReporter
    {
        private int WarningCount;
        private int ErrorCount;

        [ExcludeFromCodeCoverage]
        public virtual void Report(string message)
        {
            System.Console.WriteLine("{0}", message);
        }

        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            Report(string.Format("\nLinted {0} files in {1} seconds\n\n{2} Errors.\n{3} Warnings", fileCount, timespan.TotalSeconds, ErrorCount, WarningCount));
        }

        public void ReportViolation(IRuleViolation violation)
        {
            switch (violation.Severity)
            {
                case RuleViolationSeverity.Warning:
                    WarningCount++;
                    break;
                case RuleViolationSeverity.Error:
                    ErrorCount++;
                    break;
                case RuleViolationSeverity.Off:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ReportViolation(
                violation.FileName,
                violation.Line.ToString(),
                violation.Column.ToString(),
                violation.Severity.ToString().ToLowerInvariant(),
                violation.RuleName,
                violation.Text);
        }

        public void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText)
        {
            Report(
                string.Format(
                    "{0}({1},{2}): {3} {4} : {5}.",
                    fileName,
                    line,
                    column,
                    severity,
                    ruleName,
                    violationText));
        }
    }
}
