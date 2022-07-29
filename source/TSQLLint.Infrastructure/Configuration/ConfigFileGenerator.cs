using System.IO.Abstractions;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Configuration
{
    public class ConfigFileGenerator : IConfigFileGenerator
    {
        private const string DefaultConfigurationString =
@"{
    ""rules"": {
        ""case-sensitive-variables"": ""error"",
        ""conditional-begin-end"": ""error"",
        ""count-star"": ""error"",
        ""cross-database-transaction"": ""error"",
        ""data-compression"": ""error"",
        ""data-type-length"": ""error"",
        ""delete-where"": ""error"",
        ""disallow-cursors"": ""error"",
        ""duplicate-empty-line"" : ""error"",
        ""duplicate-go"" : ""error"",
        ""full-text"": ""error"",
        ""information-schema"": ""error"",
        ""keyword-capitalization"": ""error"",
        ""linked-server"": ""error"",
        ""multi-table-alias"": ""error"",
        ""named-constraint"": ""error"",
        ""non-sargable"": ""error"",
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
        ""unicode-string"" : ""error"",
        ""update-where"" : ""error"",
        ""upper-lower"": ""error""
    },
    ""compatibility-level"": 120
}";

        private readonly IFileSystem fileSystem;

        public ConfigFileGenerator()
            : this(new FileSystem()) { }

        public ConfigFileGenerator(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string GetDefaultConfigRules()
        {
            return DefaultConfigurationString;
        }

        public void WriteConfigFile(string path)
        {
            fileSystem.File.WriteAllText(path, DefaultConfigurationString);
        }
    }
}
