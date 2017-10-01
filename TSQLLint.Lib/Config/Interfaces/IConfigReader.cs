using TSQLLint.Common;

namespace TSQLLint.Lib.Config.Interfaces
{
    public interface IConfigReader
    {
        void LoadConfigFromFile(string configFilePath);

        void LoadConfigFromRules(string jsonConfigString);

        RuleViolationSeverity GetRuleSeverity(string key);
    }
}