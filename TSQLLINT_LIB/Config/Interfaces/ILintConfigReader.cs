using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface ILintConfigReader
    {
        RuleViolationSeverity GetRuleSeverity(string key);
        bool ConfigIsValid { get; }
    }
}