using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Console.Reporters;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console
{
    public class Application
    {
        private readonly string[] _args;
        private readonly IReporter _reporter;
        private readonly IConfigReader _configReader;
        private readonly IConfigFileFinder _configFileFinder;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly CommandLineOptions.CommandLineOptions _commandLineOptions;
        private readonly ICommandLineOptionHandler _commandLineOptionHandler;
        private readonly ConsoleTimer _timer = new ConsoleTimer();

        public Application(string[] args, IReporter reporter)
        {
            _timer.Start();
            _args = args;
            _reporter = reporter;
            _configFileFinder = new ConfigFileFinder();
            _configReader = new ConfigReader(_reporter);
            _configFileGenerator = new ConfigFileGenerator(_reporter);
            _commandLineOptions = new CommandLineOptions.CommandLineOptions(_args);
            _commandLineOptionHandler = new CommandLineOptionHandler(_commandLineOptions, _configFileFinder, _configFileGenerator, _reporter);
        }

        public void Run()
        {
            // parse options
            if (!_commandLineOptionHandler.HandleCommandLineOptions())
            {
                return;
            }

            // read config
            _configReader.LoadConfig(_commandLineOptions.ConfigFile, _commandLineOptions.DefaultConfigRules);

            // display list of plugins
            if (_commandLineOptions.ListPlugins)
            {
                _configReader.ListPlugins();
                return;
            }

            // perform lint
            var lintingHandler = new LintingHandler(_commandLineOptions, _configReader, _reporter);
            lintingHandler.Lint();

            _reporter.ReportResults(_timer.Stop(), lintingHandler.LintedFileCount);
        }
    }
}
