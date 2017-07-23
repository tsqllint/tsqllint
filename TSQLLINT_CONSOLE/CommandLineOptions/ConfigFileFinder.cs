using System.IO;
using TSQLLINT_CONSOLE.CommandLineOptions.Interfaces;

namespace TSQLLINT_CONSOLE.CommandLineOptions
{
    public class ConfigFileFinder : IConfigFileFinder
    {
        public bool FindFile(string configFile)
        {
            return File.Exists(configFile);
        }
    }
}