using System.Collections.Generic;

namespace TSQLLint.Console.Standard.Interfaces
{
    public interface ICommandLineOptions
    {
        string ConfigFile { get; set; }
        
        bool Force { get; set; }
        
        bool Help { get; set; }
        
        bool Init { get; set; }
        
        IEnumerable<string> LintPath { get; set; }
        
        bool ListPlugins { get; set; }
        
        bool PrintConfig { get; set; }
        
        bool Version { get; set; }

        string GetUsage();
    }
}
