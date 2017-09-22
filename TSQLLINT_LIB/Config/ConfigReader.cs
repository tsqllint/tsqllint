using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public class ConfigReader : IConfigReader
    {
        private readonly Dictionary<string, RuleViolationSeverity> _rules = new Dictionary<string, RuleViolationSeverity>();
        public bool ConfigIsValid { get; protected set; }

        public void LoadConfigFromFile(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath) || !File.Exists(configFilePath)) return;

            var jsonConfigString = File.ReadAllText(configFilePath);

            LoadConfigFromRules(jsonConfigString);
        }

        public void LoadConfigFromRules(string jsonConfigString)
        {
            if (string.IsNullOrEmpty(jsonConfigString)) return;

            JToken token;
            if (Utility.Utility.TryParseJson(jsonConfigString, out token))
            {
                SetupRules(token);
            }
        }

        private void SetupRules(JToken jsonObject)
        {
            ConfigIsValid = true;
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

                    _rules.Add(prop.Name, severity);
                }
            }
        }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            RuleViolationSeverity ruleValue;
            return _rules.TryGetValue(key, out ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }
    }
}