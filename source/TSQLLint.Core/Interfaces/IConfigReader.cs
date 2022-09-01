using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface IConfigReader
    {
        bool IsConfigLoaded { get; }

        int CompatabilityLevel { get; }

        string ConfigFileLoadedFrom { get; }

        Dictionary<string, string> GetPlugins();

        RuleViolationSeverity GetRuleSeverity(string key, RuleViolationSeverity defaultValue = RuleViolationSeverity.Off);

        void ListPlugins();

        void LoadConfig(string configFilePath);
    }
}
