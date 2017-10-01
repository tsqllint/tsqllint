using System;
using System.IO;
using TSQLLint.Console.ConfigHandler.Interfaces;

namespace TSQLLint.Console.ConfigHandler
{
    public class ConfigFileFinder : IConfigFileFinder
    {
        public string DefaultConfigFileName
        {
            get
            {
                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(usersDirectory, @".tsqllintrc");
            }
        }

        public bool FindFile(string configFile)
        {
            return File.Exists(configFile);
        }
    }
}