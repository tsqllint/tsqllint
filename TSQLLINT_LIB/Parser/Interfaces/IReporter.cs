using System;
using System.Collections.Generic;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface IReporter
    {
        void ReportResults(List<RuleViolation> violations, TimeSpan timespan, int fileCount);
        void Report(string message);
    }
}