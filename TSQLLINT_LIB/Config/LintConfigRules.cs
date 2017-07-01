using Newtonsoft.Json;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Config
{
    internal class LintConfigRules
    {
        [JsonProperty("select-star")]
        public RuleViolationSeverity SelectStar { get; set; }

        [JsonProperty("statement-semicolon-termination")]
        public RuleViolationSeverity StatementSemicolon { get; set; }

        [JsonProperty("set-transaction-isolation-level")]
        public RuleViolationSeverity SetTransactionIsolationLevel { get; set; }

        [JsonProperty("set-nocount")]
        public RuleViolationSeverity SetNoCount { get; set; }

        [JsonProperty("schema-qualify")]
        public RuleViolationSeverity SchemaQualify { get; set; }

        [JsonProperty("information-schema")]
        public RuleViolationSeverity InformationSchema { get; set; }
    }
}