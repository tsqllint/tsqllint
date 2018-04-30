using System;
using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class CreateConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IConfigFileGenerator configFileGenerator;
        private readonly IFileSystem fileSystem;
        private readonly string defaultConfigFilePath;

        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator, IFileSystem fileSystem)
        {
            this.reporter = reporter;
            this.configFileGenerator = configFileGenerator;
            this.fileSystem = fileSystem;
            defaultConfigFilePath = fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            var configFileExists = fileSystem.File.Exists(defaultConfigFilePath);

            if (!configFileExists || commandLineOptions.Force)
            {
                CreateConfigFile();
                reporter.Report($@"Created default config file at {defaultConfigFilePath}");
            }
            else
            {
                reporter.Report($"Default config file already exists at: {defaultConfigFilePath} use the '--init' option combined with the '--force' option to overwrite");
            }
        }

        private void CreateConfigFile()
        {
            configFileGenerator.WriteConfigFile(defaultConfigFilePath);
        }
    }
}
