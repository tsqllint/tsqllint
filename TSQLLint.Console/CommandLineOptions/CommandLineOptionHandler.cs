using System.Linq;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies;
using TSQLLint.Console.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions
{
    public class CommandLineOptionHandler : ICommandLineOptionHandler
    {
        private readonly CommandLineOptions commandLineOptions;
        private readonly IConfigFileGenerator configFileGenerator;
        private readonly IBaseReporter reporter;
        private readonly IConfigReader configReader;

        public CommandLineOptionHandler(
            CommandLineOptions commandLineOptions,
            IConfigFileGenerator configFileGenerator,
            IConfigReader configReader,
            IBaseReporter reporter)
        {
            this.commandLineOptions = commandLineOptions;
            this.configFileGenerator = configFileGenerator;
            this.configReader = configReader;
            this.reporter = reporter;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
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
                var strategy = new LoadConfigFileStrategy(reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.Init)
            {
                var strategy = new CreateConfigFileStrategy(reporter, configFileGenerator);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (this.commandLineOptions.ListPlugins)
            {
                var strategy = new PrintPluginsStrategy(reporter, configReader);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (!this.commandLineOptions.LintPath.Any())
            {
                var strategy = new PrintUsageStrategy(reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
        }
    }
}
