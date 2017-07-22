using System.IO;

namespace TSQLLINT_CONSOLE.CommandLineOptions
{
    public class ConfigFileFinder : IConfigFileFinder
    {
        public bool FindDefaultConfigFile(string configFile)
        {
            return File.Exists(configFile);
        }
    }
}