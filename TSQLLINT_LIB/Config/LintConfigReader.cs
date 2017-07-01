using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
            // deserialize string into lint config poco
            var lintConfig = JsonConvert.DeserializeObject<LintConfig>(jsonConfig);

            // add rule configurations to kvp list
            foreach (var prop in typeof(LintConfigRules).GetProperties())
            {
                var attrs = (JsonPropertyAttribute[])prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
                foreach (var attr in attrs)
                {
                    Rules.Add(attr.PropertyName, (RuleViolationSeverity)prop.GetValue(lintConfig.Rules));
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
