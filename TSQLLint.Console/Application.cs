using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions;
using TSQLLint.Console.Reporters;
using TSQLLint.Lib.Config;

namespace TSQLLint.Console
{
    public class Application
    {
        private readonly string[] _args;
        private readonly IReporter _reporter;
        private readonly ConsoleTimer _timer = new ConsoleTimer();

        public Application(string[] args, IReporter reporter)
        {
            _timer.Start();
            _args = args;
            _reporter = reporter;
        }

        public void Run()
        {
            // parse options
            var commandLineOptions = new CommandLineOptions.CommandLineOptions(_args);

            // perform non-linting actions
            var configHandler = new ConfigHandler(commandLineOptions, _reporter);
            if (!configHandler.HandleConfigs())
            {
                return;
            }

            // read config
            var configReader = new ConfigReader(_reporter);
            configReader.LoadConfig(commandLineOptions.ConfigFile, commandLineOptions.DefaultConfigRules);

            // display list of plugins
            if (commandLineOptions.ListPlugins)
            {
                configReader.ListPlugins();
                return;
            }

            // perform lint
            var lintingHandler = new LintingHandler(commandLineOptions, configReader, _reporter);
            lintingHandler.Lint();

            _reporter.ReportResults(_timer.Stop(), lintingHandler.LintedFileCount);
        }
    }
}
