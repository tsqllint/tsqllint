using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console
{
    public class CommandLineOptionHandler : ICommandLineOptionHandler
    {
        private readonly IConfigFileGenerator configFileGenerator;
        private readonly IBaseReporter reporter;
        private readonly IConfigReader configReader;
        private readonly IFileSystemWrapper fileSystemWrapper;

        public CommandLineOptionHandler(
            IConfigFileGenerator configFileGenerator,
            IConfigReader configReader,
            IBaseReporter reporter,
            IFileSystemWrapper fileSystemWrapper)
        {
            this.configFileGenerator = configFileGenerator;
            this.configReader = configReader;
            this.reporter = reporter;
            this.fileSystemWrapper = fileSystemWrapper;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            if (commandLineOptions.Args.Length == 0 || commandLineOptions.Help)
            {
                var strategy = new PrintUsageStrategy(reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.Version)
            {
                var strategy = new PrintVersionStrategy(reporter);
                strategy.HandleCommandLineOptions();
            }
            else if (commandLineOptions.PrintConfig)
            {
                var strategy = new PrintConfigStrategy(reporter, configReader);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (!string.IsNullOrWhiteSpace(commandLineOptions.ConfigFile))
            {
                var strategy = new LoadConfigFileStrategy(reporter, fileSystemWrapper);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.Init)
            {
                var strategy = new CreateConfigFileStrategy(reporter, configFileGenerator, fileSystemWrapper);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.ListPlugins)
            {
                var strategy = new PrintPluginsStrategy(reporter, configReader);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (!commandLineOptions.LintPath.Any())
            {
                var strategy = new PrintUsageStrategy(reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
        }
    }
}