using System;
using System.Diagnostics.CodeAnalysis;

namespace TSQLLint.Console.ConfigHandler
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class TSQLLintOption : Attribute
    {
        public bool NonLintingCommand { get; set; }
    }  
}
