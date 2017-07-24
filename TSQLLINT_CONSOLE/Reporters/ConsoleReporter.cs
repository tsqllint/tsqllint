using System;
using System.Collections.Generic;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_CONSOLE.Reporters
{
    public class ConsoleReporter : IReporter
    {
        public void ReportResults(List<RuleViolation> violations, TimeSpan timespan, int fileCount)
        {
            var errorCount = 0;
            var warningCount = 0;

            foreach (var violation in violations)
            {
                switch (violation.Severity)
                {
                    case RuleViolationSeverity.Warning:
                        warningCount++;
                        break;
                    case RuleViolationSeverity.Error:
                        errorCount++;
                        break;
                    case RuleViolationSeverity.Off:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ReportViolation(string.Format("{0}({1},{2}): {3} {4} : {5}.",
                    violation.FileName, 
                    violation.Line, 
                    violation.Column,
                    violation.Severity.ToString().ToLowerInvariant(),
                    violation.RuleName, 
                    violation.Text));
            }

            Report(string.Format("\nLinted {0} files in {1} seconds\n\n{2} Errors.\n{3} Warnings", fileCount, timespan.TotalSeconds, errorCount, warningCount));
        }

        private static void ReportViolation(string message)
        {
            Console.WriteLine("{0}", message);
        }

        public void Report(string message)
        {
            Console.WriteLine("{0}", message);
        }
    }
}