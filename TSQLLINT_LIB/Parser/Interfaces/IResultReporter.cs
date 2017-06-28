using System.Collections.Generic;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface IResultReporter
    {
        void ReportResults(List<RuleViolation> violations);
    }
}