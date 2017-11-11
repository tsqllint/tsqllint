using System;
using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class CreateConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IFileSystem _fileSystem;
        private readonly string _configFilePath;

        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator) : this(reporter, configFileGenerator, new FileSystem()) { }
            
        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator, IFileSystem fileSystem)
        {
            _reporter = reporter;
            _configFileGenerator = configFileGenerator;
            _fileSystem = fileSystem;
            _configFilePath = _fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
        }

        private void CreateConfigFile()
        {
            _configFileGenerator.WriteConfigFile(_configFilePath);
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            var configFileExists = _fileSystem.File.Exists(_configFilePath);
            
            if (!configFileExists)
            {
                CreateConfigFile();
            }
            else if (commandLineOptions.Force)
            {
                CreateConfigFile();
            }
            else
            {
                _reporter.Report($"Default config file already exists at: {_configFilePath} use the '--init' option combined with the '--force' option to overwrite");
            }
        }
    }
}
