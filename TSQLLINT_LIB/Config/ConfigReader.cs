using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config.Interfaces;

namespace TSQLLINT_LIB.Config
{
    public class ConfigReader : IConfigReader
    {
        public bool ConfigIsValid { get; protected set; }

        private readonly Dictionary<string, RuleViolationSeverity> Rules = new Dictionary<string, RuleViolationSeverity>();
        private readonly Dictionary<string, string> PluginPaths = new Dictionary<string, string>();

        public ConfigReader(IReporter reporter, string configFilePath) : this(reporter, new FileSystem(), configFilePath)
        {
        }

        public ConfigReader(IReporter reporter, IFileSystem fileSystem, string configFilePath)
        {

            if (!string.IsNullOrEmpty(configFilePath) && fileSystem.File.Exists(configFilePath))
            {
                var jsonConfigString = fileSystem.File.ReadAllText(configFilePath);

                JToken token;
                if (Utility.Utility.TryParseJson(jsonConfigString, out token))
                {
                    ConfigIsValid = true;
                    SetupRules(token);
                    SetupPlugins(token);
                }
                else
                {
                    reporter.Report("Config file is not valid Json.");
                }
            }
        }

        private void SetupPlugins(JToken jsonObject)
        {
            var rules = jsonObject.SelectTokens("..plugins").ToList();

            for (var index = 0; index < rules.Count; index++)
            {
                var rule = rules[index];
                foreach (var jToken in rule.Children())
                {
                    var prop = (JProperty)jToken;
                    PluginPaths.Add(prop.Name, prop.Value.ToString());
                }
            }
        }

        private void SetupRules(JToken jsonObject)
        {
            var rules = jsonObject.SelectTokens("..rules").ToList();

            for (var index = 0; index < rules.Count; index++)
            {
                var rule = rules[index];
                foreach (var jToken in rule.Children())
                {
                    var prop = (JProperty) jToken;

                    RuleViolationSeverity severity;
                    if (!Enum.TryParse(prop.Value.ToString(), true, out severity))
                    {
                        continue;
                    }

                    Rules.Add(prop.Name, severity);
                }
            }
        }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            RuleViolationSeverity ruleValue;
            return Rules.TryGetValue(key, out ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }

        public Dictionary<string, string> GetPlugins()
        {
            return PluginPaths;
        }
    }
}