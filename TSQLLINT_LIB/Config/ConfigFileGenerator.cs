using System.IO;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;

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
        private readonly IBaseReporter reporter;

        public ConfigFileGenerator(IBaseReporter reporter)
        {
            this.reporter = reporter;
        }

        public void WriteConfigFile(string path)
        {
            File.WriteAllText(path, ConfigString);
            this.reporter.Report(string.Format("Created default config file {0}", path));
        }
    }
}