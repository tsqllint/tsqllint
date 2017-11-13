using System.Linq;
using TSQLLint.Common;
using TSQLLint.Console.Standard.CommandLineOptions.CommandLineOptionStrategies;
using TSQLLint.Console.Standard.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.Standard.CommandLineOptions
{
    public class CommandLineOptionHandler : ICommandLineOptionHandler
    {
        private readonly ICommandLineOptions _options;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IBaseReporter _reporter;
        private readonly IConfigReader _configReader;

        public CommandLineOptionHandler(
            ICommandLineOptions options,
            IConfigFileGenerator configFileGenerator,
            IConfigReader configReader,
            IBaseReporter reporter)
        {
            _options = options;
            _configFileGenerator = configFileGenerator;
            _configReader = configReader;
            _reporter = reporter;
        }

        public void HandleCommandLineOptions(ICommandLineOptions options)
        {
            if (options.Help)
            {
                var strategy = new PrintUsageStrategy(_reporter);
                strategy.HandleCommandLineOptions(options);
            }
            else if (options.Version)
            {
                var strategy = new PrintVersionStrategy(_reporter);
                strategy.HandleCommandLineOptions();
            }
            else if (options.PrintConfig)
            {
                var strategy = new PrintConfigStrategy(_reporter, _configReader);
                strategy.HandleCommandLineOptions(options);
            }
            else if (!string.IsNullOrWhiteSpace(options.ConfigFile))
            {
                var strategy = new LoadConfigFileStrategy(_reporter);
                strategy.HandleCommandLineOptions(options);
            }
            else if (options.Init)
            {
                var strategy = new CreateConfigFileStrategy(_reporter, _configFileGenerator);
                strategy.HandleCommandLineOptions(options);
            }
            else if (_options.ListPlugins)
            {
                var strategy = new PrintPluginsStrategy(_reporter, _configReader);
                strategy.HandleCommandLineOptions(options);
            }
            else if (!_options.LintPath.Any())
            {
                var strategy = new PrintUsageStrategy(_reporter);
                strategy.HandleCommandLineOptions(options);
            }
        }
    }
}
