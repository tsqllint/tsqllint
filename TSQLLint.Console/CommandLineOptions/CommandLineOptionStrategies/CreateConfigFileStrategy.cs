using System;
using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Console.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class CreateConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IFileSystem _fileSystem;
        private readonly string _defaultConfigFilePath;

        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator) : this(reporter, configFileGenerator, new FileSystem()) { }
            
        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator, IFileSystem fileSystem)
        {
            _reporter = reporter;
            _configFileGenerator = configFileGenerator;
            _fileSystem = fileSystem;
            _defaultConfigFilePath = _fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
        }

        private void CreateConfigFile()
        {
            _configFileGenerator.WriteConfigFile(_defaultConfigFilePath);
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            var configFileExists = _fileSystem.File.Exists(_defaultConfigFilePath);
            
            if (!configFileExists || commandLineOptions.Force)
            {
                CreateConfigFile();
                _reporter.Report($@"Created default config file at {_defaultConfigFilePath}");
            }
            else
            {
                _reporter.Report($"Default config file already exists at: {_defaultConfigFilePath} use the '--init' option combined with the '--force' option to overwrite");
            }
        }
    }
}
