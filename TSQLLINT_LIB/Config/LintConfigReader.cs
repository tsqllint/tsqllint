using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public class LintConfigReader : ILintConfigReader
    {
        public Dictionary<string, RuleViolationSeverity> Rules { get; private set; }

        public LintConfigReader(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new Exception("Config file not valid");
            }

            Rules = new Dictionary<string, RuleViolationSeverity>();
            var jsonConfig = File.ReadAllText(configFilePath);
            SetupRules(jsonConfig);
        }

        private void SetupRules(string jsonConfig)
        {
            var jsonObject = JObject.Parse(jsonConfig);

            JToken rules;
            var rulesFound = jsonObject.TryGetValue("rules", out rules);

            if (!rulesFound)
            {
                return;
            }

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
