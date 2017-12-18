using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Lib.Config.Interfaces
{
    public interface IConfigReader
    {
        bool IsConfigLoaded { get; }

        string ConfigFileLoadedFrom { get; }

        Dictionary<string, string> GetPlugins();

        RuleViolationSeverity GetRuleSeverity(string key);

        void ListPlugins();

        void LoadConfig(string configFilePath);
    }
}
