using System;
using System.Collections.Generic;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface IBaseReporter
    {
        void Report(string message);
    }

    public interface IReporter : IBaseReporter
    {
        void ReportResults(List<RuleViolation> violations, TimeSpan timespan, int fileCount);
    }
}