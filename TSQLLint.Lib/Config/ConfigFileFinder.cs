using System;
using System.IO.Abstractions;

namespace TSQLLint.Lib.Config
{
    public class ConfigFileFinder : IConfigFileFinder
    {
        private readonly IFileSystem _fileSystem;

        public ConfigFileFinder() : this(new FileSystem()) { }
        
        public ConfigFileFinder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public string DefaultConfgigFilePath
        {
            get
            {
                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return _fileSystem.Path.Combine(usersDirectory, @".tsqllintrc");
            }
        }

        public bool FindFile(string configFile)
        {
            return _fileSystem.File.Exists(configFile);
        }
    }
}
