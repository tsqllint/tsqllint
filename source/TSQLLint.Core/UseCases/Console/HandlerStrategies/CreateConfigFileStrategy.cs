using System;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
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

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
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

            return new HandlerResponseMessage(true, false);
        }

        private void CreateConfigFile()
        {
            configFileGenerator.WriteConfigFile(defaultConfigFilePath);
        }
    }
}
