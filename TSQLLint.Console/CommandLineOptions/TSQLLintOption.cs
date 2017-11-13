using System;
using System.Diagnostics.CodeAnalysis;

namespace TSQLLint.Console.CommandLineOptions
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class TSQLLintOption : Attribute
    {
        public bool NonLintingCommand { get; set; }
    }  
}
