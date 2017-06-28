using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public interface ILintConfigReader
    {
        RuleViolationSeverity GetRuleSeverity(string key);
    }
}