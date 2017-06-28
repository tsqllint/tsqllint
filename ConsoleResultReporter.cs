using System;
using System.Collections.Generic;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT
{
    public class ConsoleResultReporter : IResultReporter
    {
        public void ReportResults(List<RuleViolation> violations)
        {
            foreach (var violation in violations)
            {
                Console.WriteLine("{0}({1},{2}): {3} {4} : {5}.",
                    violation.FileName, violation.Line, violation.Column,
                    violation.Severity.ToString().ToLowerInvariant(),
                    violation.RuleName, violation.Text);
            }
        }
    }
}

