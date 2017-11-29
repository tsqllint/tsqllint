using System.IO.Abstractions;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Lib.Config
{
    public class ConfigFileGenerator : IConfigFileGenerator
    {
        private const string ConfigString =
@"{
    ""rules"": {
        ""concat-strings"": ""error"",
        ""conditional-begin-end"": ""error"",
        ""cross-database"": ""error"",
        ""cross-database-transaction"": ""error"",
        ""data-compression"": ""error"",
        ""data-type-length"": ""error"",
        ""disallow-cursors"": ""error"",
        ""full-text"": ""error"",
        ""information-schema"": ""error"",
        ""keyword-capitalization"": ""error"",
        ""linked-server"": ""error"",
        ""multi-table-alias"": ""error"",
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
        ""upper-lower"": ""error""
    }
}";

        private readonly IFileSystem _fileSystem;

        public ConfigFileGenerator() : this(new FileSystem()) { }

        public ConfigFileGenerator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string GetDefaultConfigRules()
        {
            return ConfigString;
        }

        public void WriteConfigFile(string path)
        {
            _fileSystem.File.WriteAllText(path, ConfigString);
        }
    }
}
