using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class PrintPluginsStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigReader _configReader;

        public PrintPluginsStrategy(IBaseReporter reporter, IConfigReader configReader)
        {
            _reporter = reporter;
            _configReader = configReader;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            if (_configReader.IsConfigLoaded)
            {
                _configReader.ListPlugins();
            }
            else
            {
                _reporter.Report("No Plugins Found");
            }
        }
    }
}
