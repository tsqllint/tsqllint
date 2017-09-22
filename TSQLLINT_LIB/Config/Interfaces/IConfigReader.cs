using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface IConfigReader
    {
        bool ConfigIsValid { get; }
        void LoadConfigFromFile(string configFilePath);
        void LoadConfigFromRules(string jsonConfigString);
        RuleViolationSeverity GetRuleSeverity(string key);
    }
}