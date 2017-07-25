using System;
using System.Diagnostics.CodeAnalysis;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class TSQLLINTOption : Attribute
    {
        public bool NonLintingCommand { get; set; }
    }  
}
