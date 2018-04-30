using System;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console
{
    public class CreateConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IConfigFileGenerator configFileGenerator;
        private readonly IFileSystemWrapper fileSystem;
        private readonly string defaultConfigFilePath;

        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileGenerator configFileGenerator, IFileSystemWrapper fileSystem)
        {
            this.reporter = reporter;
            this.configFileGenerator = configFileGenerator;
            this.fileSystem = fileSystem;
            defaultConfigFilePath = fileSystem.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            var configFileExists = fileSystem.FileExists(defaultConfigFilePath);

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