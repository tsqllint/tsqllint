using System.Collections.Generic;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public interface ILintConfigReader
    {
        Dictionary<string, RuleViolationSeverity> Rules { get; }

        RuleViolationSeverity GetRuleSeverity(string key);
    }
}