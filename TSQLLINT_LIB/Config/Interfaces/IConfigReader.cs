using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface IConfigReader
    {
        bool ConfigIsValid { get; }
        RuleViolationSeverity GetRuleSeverity(string key);
    }
}