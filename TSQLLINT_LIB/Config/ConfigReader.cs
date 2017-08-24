using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public class ConfigReader : IConfigReader
    {
        public bool ConfigIsValid { get; protected set; }
        private readonly Dictionary<string, RuleViolationSeverity> Rules = new Dictionary<string, RuleViolationSeverity>();

        public ConfigReader(string configFilePath)
        {
            if (!string.IsNullOrEmpty(configFilePath) && File.Exists(configFilePath))
            {
                var jsonConfigString = File.ReadAllText(configFilePath);

                JToken token;
                if (Utility.Utility.TryParseJson(jsonConfigString, out token))
                {
                    SetupRules(token);
                }
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

                    Rules.Add(prop.Name, severity);
                }
            }
        }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            RuleViolationSeverity ruleValue;
            return Rules.TryGetValue(key, out ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }
    }
}