using System;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_CONSOLE.Reporters
{
    public class ConsoleReporter : IReporter
    {
        private int WarningCount;
        private int ErrorCount;

        public void Report(string message)
        {
            Console.WriteLine("{0}", message);
        }

        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            Report(string.Format("\nLinted {0} files in {1} seconds\n\n{2} Errors.\n{3} Warnings", fileCount, timespan.TotalSeconds, ErrorCount, WarningCount));
        }

        public void ReportViolation(RuleViolation violation)
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

            Report(string.Format("{0}({1},{2}): {3} {4} : {5}.",
                violation.FileName,
                violation.Line,
                violation.Column,
                violation.Severity.ToString().ToLowerInvariant(),
                violation.RuleName,
                violation.Text));
        }
    }
}