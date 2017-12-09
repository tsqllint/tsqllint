using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Newtonsoft.Json.Linq;
using TSQLLint.Common;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Lib.Config
{
    public class ConfigReader : IConfigReader
    {
        private readonly Dictionary<string, RuleViolationSeverity> _rules = new Dictionary<string, RuleViolationSeverity>();

        private readonly Dictionary<string, string> _pluginPaths = new Dictionary<string, string>();

        private readonly IReporter _reporter;

        private readonly IFileSystem _fileSystem;

        public string ConfigFileLoadedFrom { get; private set; }

        public ConfigReader(IReporter reporter) : this(reporter, new FileSystem()) { }

        public ConfigReader(IReporter reporter, IFileSystem fileSystem)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
        }

        public bool IsConfigLoaded { get; private set; }

        private void SetupPlugins(JToken jsonObject)
        {
            var plugins = jsonObject.SelectTokens("..plugins").ToList();

            foreach (var plugin in plugins)
            {
                foreach (var jToken in plugin.Children())
                {
                    var prop = (JProperty)jToken;
                    _pluginPaths.Add(prop.Name, prop.Value.ToString());
                }
            }
        }

        private void SetupRules(JToken jsonObject)
        {
            var rules = jsonObject.SelectTokens("..rules").ToList();

            foreach (var rule in rules)
            {
                foreach (var jToken in rule.Children())
                {
                    var prop = (JProperty)jToken;

                    if (!Enum.TryParse(prop.Value.ToString(), true, out RuleViolationSeverity severity))
                    {
                        continue;
                    }

                    _rules.Add(prop.Name, severity);
                }
            }
        }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            return _rules.TryGetValue(key, out var ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }

        public Dictionary<string, string> GetPlugins()
        {
            return _pluginPaths;
        }

        public void LoadConfig(string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                var defaultConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
                if (_fileSystem.File.Exists(defaultConfigFilePath))
                {
                    LoadConfigFromFile(defaultConfigFilePath);
                    return;
                }

                // load in memory config
                var configFileGenerator = new ConfigFileGenerator();
                LoadConfigFromJson(configFileGenerator.GetDefaultConfigRules());
            }
            else
            {
                LoadConfigFromFile(configFilePath);
            }
        }

        private void LoadConfigFromFile(string configFilePath)
        {
            if (_fileSystem.File.Exists(configFilePath))
            {
                var jsonConfigString = _fileSystem.File.ReadAllText(configFilePath);
                LoadConfigFromJson(jsonConfigString);
                ConfigFileLoadedFrom = configFilePath;
            }
            else
            {
                _reporter.Report($@"Config file not found: {configFilePath}");
            }
        }

        private void LoadConfigFromJson(string jsonConfigString)
        {
            if (Utility.ParsingUtility.TryParseJson(jsonConfigString, out var token))
            {
                SetupRules(token);
                SetupPlugins(token);
                IsConfigLoaded = true;
            }
            else
            {
                _reporter.Report("Config file is not valid Json.");
            }
        }

        public void ListPlugins()
        {
            var havePlugins = IsConfigLoaded && _pluginPaths.Count > 0;
            if (havePlugins)
            {
                _reporter.Report("Found the following plugins:");
                foreach (var plugin in _pluginPaths)
                {
                    _reporter.Report($"Plugin Name '{plugin.Key}' loaded from path '{plugin.Value}'");
                }
            }
            else
            {
                _reporter.Report("Did not find any plugins");
            }
        }
    }
}
