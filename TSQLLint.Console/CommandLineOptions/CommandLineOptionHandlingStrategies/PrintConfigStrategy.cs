using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class PrintConfigStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigReader _configReader;

        public PrintConfigStrategy(IBaseReporter reporter, IConfigReader configReader)
        {
            _reporter = reporter;
            _configReader = configReader;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            _reporter.Report(string.IsNullOrEmpty(_configReader.ConfigFileLoadedFrom)
                    ? "Using default in memory config." : $"Config file found at: {_configReader.ConfigFileLoadedFrom}");
        }
    }
}
