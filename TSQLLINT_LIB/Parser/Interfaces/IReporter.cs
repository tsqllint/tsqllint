using System;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface IBaseReporter
    {
        void Report(string message);
    }

    public interface IReporter : IBaseReporter
    {
        void ReportResults(TimeSpan timespan, int fileCount);
        void ReportViolation(RuleViolation violation);
    }
}