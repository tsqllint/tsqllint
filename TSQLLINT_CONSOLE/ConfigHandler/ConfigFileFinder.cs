using System.IO;
using TSQLLINT_CONSOLE.ConfigHandler.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class ConfigFileFinder : IConfigFileFinder
    {
        public bool FindFile(string configFile)
        {
            return File.Exists(configFile);
        }
    }
}