using Newtonsoft.Json;

namespace TSQLLINT_LIB.Config
{
    internal class LintConfig
    {
        [JsonProperty("rules")]
        public LintConfigRules Rules { get; set; }
    }
}