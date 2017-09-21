using TSQLLINT_COMMON;

namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface IConfigReader
    {
        RuleViolationSeverity GetRuleSeverity(string key);
        bool ConfigIsValid { get; }
    }
}