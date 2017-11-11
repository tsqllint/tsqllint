using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Lib.Config.Interfaces
{
    public interface IConfigReader
    {
        bool IsConfigLoaded { get; }

        Dictionary<string, string> GetPlugins();
        
        RuleViolationSeverity GetRuleSeverity(string key);
        
        void ListPlugins();
        
        void LoadConfig(string configFile, string defaultConfigRules);
        
        void LoadConfigFromFile(string configFilePath);
        
        void LoadConfigFromRules(string jsonConfigString);
    }
}
