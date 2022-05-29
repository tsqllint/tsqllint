using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Reporters
{
    public class ConsoleReporter : IConsoleReporter
    {
        public bool ReporterMuted { get; set; }
        public int? FixedCount { get; set; }
        private readonly ConcurrentBag<IRuleViolation> ruleViolations = new();
        private int errorCount;
        private int warningCount;

        public bool ShouldCollectViolations { get; set; }

        public List<IRuleViolation> Violations => ruleViolations.ToList();

        public void ClearViolations()
        {
            ruleViolations.Clear();
        }

        [ExcludeFromCodeCoverage]
        public virtual void Report(string message)
        {
            NonBlockingConsole.WriteLine(message);
        }

        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            var text = $"\nLinted {fileCount} files in {timespan.TotalSeconds} seconds\n\n{errorCount} Errors.\n{warningCount} Warnings";
            if (FixedCount > 0)
            {
                text += $"\n\nFixed {FixedCount}";
            }

            Report(text);
        }

        public void ReportFileResults()
        {
        }

        public void ReportViolation(IRuleViolation violation)
        {
            if (ShouldCollectViolations)
            {
                ruleViolations.Add(violation);
            }

            // If --fix is turned on sometimes the program runs a couple times to fixed issues
            // caused by fixing another rule. This prevents double or tripple counting.
            if (!ReporterMuted)
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

                ReportViolation(
                    violation.FileName,
                    violation.Line.ToString(),
                    violation.Column.ToString(),
                    violation.Severity.ToString().ToLowerInvariant(),
                    violation.RuleName,
                    violation.Text);
            }
        }

        public void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText)
        {
            Report($"{fileName}({line},{column}): {severity} {ruleName} : {violationText}.");
        }
    }
}
