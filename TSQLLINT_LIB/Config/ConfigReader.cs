using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config.Interfaces;

namespace TSQLLINT_LIB.Config
{
    public class ConfigReader : IConfigReader
    {
        private readonly Dictionary<string, RuleViolationSeverity> Rules = new Dictionary<string, RuleViolationSeverity>();
        private readonly Dictionary<string, string> PluginPaths = new Dictionary<string, string>();

        private readonly IReporter _reporter;
        private readonly IFileSystem _fileSystem;

        public ConfigReader(IReporter reporter) : this(reporter, new FileSystem()) {}

        public ConfigReader(IReporter reporter, IFileSystem fileSystem)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
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

        public void LoadConfigFromFile(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath) || !_fileSystem.File.Exists(configFilePath))
            {
                return;
            }

            var jsonConfigString = _fileSystem.File.ReadAllText(configFilePath);
            LoadConfigFromRules(jsonConfigString);
        }

        public void LoadConfigFromRules(string jsonConfigString)
        {
            if (string.IsNullOrEmpty(jsonConfigString)) return;

            JToken token;
            if (Utility.Utility.TryParseJson(jsonConfigString, out token))
            {
                SetupRules(token);
                SetupPlugins(token);
            }
            else
            {
                _reporter.Report("Config file is not valid Json.");
            }
        }
    }
}
