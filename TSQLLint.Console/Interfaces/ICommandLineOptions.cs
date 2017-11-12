using System.Collections.Generic;

namespace TSQLLint.Console.Interfaces
{
    public interface ICommandLineOptions
    {
        string[] Args { get; set; }
        
        string ConfigFile { get; set; }
        
        bool Force { get; set; }
        
        bool Help { get; set; }
        
        bool Init { get; set; }
        
        List<string> LintPath { get; set; }
        
        bool ListPlugins { get; set; }
        
        bool PrintConfig { get; set; }
        
        bool Version { get; set; }

        string GetUsage();
    }
}
