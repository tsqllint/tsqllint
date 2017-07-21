using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Config
{
    public class ConfigFileGenerator
    {
        private readonly IBaseReporter Reporter;
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
        ""upper-lower"": ""error""
    }
}";

        public ConfigFileGenerator(IBaseReporter reporter)
        {
            Reporter = reporter;
        }

        public void WriteConfigFile()
        {
            System.IO.File.WriteAllText(@".tsqllintrc", ConfigString);
            Reporter.Report("Created default config file '.tsqllintrc'");
        }
    }
}