using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;

namespace TSQLLint.Lib.Reporters
{
    public class ConsoleReporter : IReporter
    {
        private int warningCount;
        private int errorCount;
        private List<IRuleViolation> violationList = new List<IRuleViolation>();

        [ExcludeFromCodeCoverage]
        public virtual void Report(string message)
        {
            System.Console.WriteLine("{0}", message);
        }

        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            Report($"\nLinted {fileCount} files in {timespan.TotalSeconds} seconds\n\n{errorCount} Errors.\n{warningCount} Warnings");
        }

        public void ReportFileResults()
        {
            violationList.Sort((x, y) =>
            {
                var v = x.Line.CompareTo(y.Line);
                if (v == 0) { v = x.Column.CompareTo(y.Column); }
                return v;
            });

            foreach (IRuleViolation violation in violationList)
            {
                ReportViolation(
                    violation.FileName,
                    violation.Line.ToString(),
                    violation.Column.ToString(),
                    violation.Severity.ToString().ToLowerInvariant(),
                    violation.RuleName,
                    violation.Text);
            }

            violationList.Clear();
            violationList.TrimExcess();
        }

        public void ReportViolation(IRuleViolation violation)
        {
            switch (violation.Severity)
            {
                case RuleViolationSeverity.Warning:
                    warningCount++;
                    break;
                case RuleViolationSeverity.Error:
                    errorCount++;
                    break;
                default:
                    return;
            }

            violationList.Add(violation);
        }

        public void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText)
        {
            Report($"{fileName}({line},{column}): {severity} {ruleName} : {violationText}.");
        }
    }
}
