using System;

namespace TSQLLINT_CONSOLE
{
    public static class ConfigFileGenerator
    {
        // write a default config file
        public static void WriteConfigFile()
        {
            string configString = @"
{
    ""rules"": {
        ""conditional-begin-end"": ""error"",
        ""data-compression"": ""error"",
        ""data-type-length"": ""error"",
        ""disallow-cursors"": ""error"",
        ""information-schema"": ""error"",
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
}
";
            System.IO.File.WriteAllText(@".tsqllintrc", configString);
            Console.WriteLine("Created default config file '.tsqllintrc'");
        }
    }
}