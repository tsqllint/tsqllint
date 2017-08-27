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
        private readonly Dictionary<string, RuleViolationSeverity> rules = new Dictionary<string, RuleViolationSeverity>();

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

        public bool ConfigIsValid { get; protected set; }

        public RuleViolationSeverity GetRuleSeverity(string key)
        {
            RuleViolationSeverity ruleValue;
            return this.rules.TryGetValue(key, out ruleValue) ? ruleValue : RuleViolationSeverity.Off;
        }

        private void SetupRules(JToken jsonObject)
        {
            ConfigIsValid = true;
            var rules = jsonObject.SelectTokens("..rules").ToList();

            for (var index = 0; index < rules.Count; index++)
            {
                var rule = rules[index];
                foreach (var jtoken in rule.Children())
                {
                    var prop = (JProperty)jtoken;

                    RuleViolationSeverity severity;
                    if (!Enum.TryParse(prop.Value.ToString(), true, out severity))
                    {
                        continue;
                    }

                    this.rules.Add(prop.Name, severity);
                }
            }
        }
    }
}