using System.Linq;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions
{
    public class CommandLineOptionHandler : ICommandLineOptionHandler
    {
        private readonly CommandLineOptions _commandLineOptions;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IBaseReporter _reporter;
        private readonly IConfigReader _configReader;

        public CommandLineOptionHandler(
            CommandLineOptions commandLineOptions,
            IConfigFileGenerator configFileGenerator,
            IConfigReader configReader,
            IBaseReporter reporter)
        {
            _commandLineOptions = commandLineOptions;
            _configFileGenerator = configFileGenerator;
            _configReader = configReader;
            _reporter = reporter;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            if (commandLineOptions.Args.Length == 0 || commandLineOptions.Help)
            {
                var strategy = new PrintUsageStrategy(_reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.Version)
            {
                var strategy = new VersionStrategy(_reporter);
                strategy.HandleCommandLineOptions();
            }
            else if (commandLineOptions.PrintConfig)
            {
                var strategy = new PrintConfigStrategy(_reporter, _configReader);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (!string.IsNullOrWhiteSpace(commandLineOptions.ConfigFile))
            {
                var strategy = new HandleConfigFileStrategy(_reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (commandLineOptions.Init)
            {
                var strategy = new CreateConfigFileStrategy(_reporter, _configFileGenerator);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (_commandLineOptions.ListPlugins)
            {
                var strategy = new PrintPluginsStrategy(_reporter, _configReader);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
            else if (!_commandLineOptions.LintPath.Any())
            {
                var strategy = new PrintUsageStrategy(_reporter);
                strategy.HandleCommandLineOptions(commandLineOptions);
            }
        }
    }
}
