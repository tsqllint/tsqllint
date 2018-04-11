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
        private readonly Dictionary<string, RuleViolationSeverity> configuredRules = new Dictionary<string, RuleViolationSeverity>();

        private readonly Dictionary<string, string> pluginPaths = new Dictionary<string, string>();

        private readonly IReporter reporter;

        private readonly IFileSystem fileSystem;

        private readonly IEnvironmentWrapper environmentWrapper;

        public ConfigReader(IReporter reporter)
            : this(reporter, new FileSystem(), new EnvironmentWrapper()) { }

        public ConfigReader(IReporter reporter, IFileSystem fileSystem, IEnvironmentWrapper environmentWrapper)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            this.environmentWrapper = environmentWrapper;
        }

        public string ConfigFileLoadedFrom { get; private set; }

        public bool IsConfigLoaded { get; private set; }

        public void ListPlugins()
        {
            var havePlugins = IsConfigLoaded && pluginPaths.Count > 0;
            if (havePlugins)
            {
                reporter.Report("Found the following plugins:");
                foreach (var plugin in pluginPaths)
                {
                    reporter.Report($"Plugin Name '{plugin.Key}' loaded from path '{plugin.Value}'");
                }
            }
            else
            {
                reporter.Report("Did not find any plugins");
            }
        }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            return configuredRules.TryGetValue(key, out var ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }

        public Dictionary<string, string> GetPlugins()
        {
            return pluginPaths;
        }

        public void LoadConfig(string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                var envVariableConfigFilePath = environmentWrapper.GetEnvironmentVariable("tsqllintrc");
                if (fileSystem.File.Exists(envVariableConfigFilePath))
                {
                    LoadConfigFromFile(envVariableConfigFilePath);
                    return;
                }

                var localConfigFilePath = Path.Combine(Environment.CurrentDirectory, @".tsqllintrc");
                if (fileSystem.File.Exists(localConfigFilePath))
                {
                    LoadConfigFromFile(localConfigFilePath);
                    return;
                }

                var defaultConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
                if (fileSystem.File.Exists(defaultConfigFilePath))
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
            if (fileSystem.File.Exists(configFilePath))
            {
                var jsonConfigString = fileSystem.File.ReadAllText(configFilePath);
                LoadConfigFromJson(jsonConfigString);
                ConfigFileLoadedFrom = configFilePath;
            }
            else
            {
                reporter.Report($@"Config file not found: {configFilePath}");
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
                reporter.Report("Config file is not valid Json.");
            }
        }

        private void SetupPlugins(JToken jsonObject)
        {
            var plugins = jsonObject.SelectTokens("..plugins").ToList();

            foreach (var plugin in plugins)
            {
                foreach (var jToken in plugin.Children())
                {
                    var prop = (JProperty)jToken;
                    pluginPaths.Add(prop.Name, prop.Value.ToString());
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

                    this.configuredRules.Add(prop.Name, severity);
                }
            }
        }
    }
}
