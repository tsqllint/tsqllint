using TSQLLINT_COMMON;

namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface IConfigReader
    {
        void LoadConfigFromFile(string configFilePath);

        void LoadConfigFromRules(string jsonConfigString);

        RuleViolationSeverity GetRuleSeverity(string key);
    }
}