using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public class LintConfigReader : ILintConfigReader
    {
        private readonly Dictionary<string, RuleViolationSeverity> Rules = new Dictionary<string, RuleViolationSeverity>();

        public LintConfigReader(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new Exception("Config file not valid");
            }

            var jsonConfig = File.ReadAllText(configFilePath);
            SetupRules(jsonConfig);
        }

        private void SetupRules(string jsonConfig)
        {
            var jsonObject = JObject.Parse(jsonConfig);

            JToken rules;
            if (!jsonObject.TryGetValue("rules", out rules)) return;

            foreach (var rule in rules)
            {
                var name = ((JProperty) rule).Name;
                var value = ((JProperty) rule).Value.ToString();

                RuleViolationSeverity severity;
                var severityIsValid = Enum.TryParse(value, true, out severity);

                if (severityIsValid)
                {
                    Rules.Add(name, severity);
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