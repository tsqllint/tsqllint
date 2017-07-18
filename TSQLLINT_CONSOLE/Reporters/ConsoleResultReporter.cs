using System;
using System.Collections.Generic;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_CONSOLE
{
    public class ConsoleResultReporter : IResultReporter
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

                Console.WriteLine("{0}({1},{2}): {3} {4} : {5}.",
                    violation.FileName, 
                    violation.Line, 
                    violation.Column,
                    violation.Severity.ToString().ToLowerInvariant(),
                    violation.RuleName, 
                    violation.Text);
            }

            Console.WriteLine("\nLinted {0} files in {1} seconds\n\n{2} Errors.\n{3} Warnings\n", fileCount, timespan.TotalSeconds, errorCount, warningCount);
        }

        public void ReportResults(List<RuleViolation> violations)
        {
            throw new NotImplementedException();
        }
    }
}