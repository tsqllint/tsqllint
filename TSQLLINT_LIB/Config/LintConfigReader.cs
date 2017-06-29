using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    public class LintConfigReader : ILintConfigReader
    {
        public Dictionary<string, RuleViolationSeverity> Rules { get; private set; }

        public LintConfigReader(string jsonConfig)
        {
           Rules = new Dictionary<string, RuleViolationSeverity>();
           SetupRules(jsonConfig);
        }

        private void SetupRules(string jsonConfig)
        {
            // deserialize string into lint config poco
            var lintConfig = JsonConvert.DeserializeObject<LintConfig>(jsonConfig);

            if (lintConfig == null)
            {
                throw new Exception("Config file not valid");
            }

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
