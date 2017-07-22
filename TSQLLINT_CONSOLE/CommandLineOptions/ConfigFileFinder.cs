using System.IO;

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