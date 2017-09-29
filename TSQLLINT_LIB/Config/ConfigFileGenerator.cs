using System.IO;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config.Interfaces;

namespace TSQLLINT_LIB.Config
{
    public class ConfigFileGenerator : IConfigFileGenerator
    {
        private const string ConfigString =
@"{
    ""rules"": {
        ""conditional-begin-end"": ""error"",
        ""data-compression"": ""error"",
        ""data-type-length"": ""error"",
        ""disallow-cursors"": ""error"",
        ""information-schema"": ""error"",
        ""keyword-capitalization"": ""error"",
        ""multi-table-alias"": ""error"",
        ""object-property"": ""error"",
        ""print-statement"": ""error"",
        ""schema-qualify"": ""error"",
        ""select-star"": ""error"",
        ""semicolon-termination"": ""error"",
        ""set-ansi"": ""error"",
        ""set-nocount"": ""error"",
        ""set-quoted-identifier"": ""error"",
        ""set-transaction-isolation-level"": ""error"",
        ""set-variable"": ""error"",
        ""upper-lower"": ""error""
    }
}";
        private readonly IBaseReporter _reporter;

        public ConfigFileGenerator(IBaseReporter reporter)
        {
            _reporter = reporter;
        }

        public string GetDefaultConfigRules()
        {
            _reporter.Report(".tsqllintrc configuration file not found, using defaults.");
            return ConfigString;
        }

        public void WriteConfigFile(string path)
        {
            File.WriteAllText(path, ConfigString);
            _reporter.Report(string.Format("Created default config file {0}.", path));
        }
    }
}