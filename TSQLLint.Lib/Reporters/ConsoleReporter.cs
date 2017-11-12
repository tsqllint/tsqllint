using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;

namespace TSQLLint.Lib.Reporters
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
            Report($"\nLinted {fileCount} files in {timespan.TotalSeconds} seconds\n\n{ErrorCount} Errors.\n{WarningCount} Warnings");
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
                default:
                    return;
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
                $"{fileName}({line},{column}): {severity} {ruleName} : {violationText}.");
        }
    }
}
