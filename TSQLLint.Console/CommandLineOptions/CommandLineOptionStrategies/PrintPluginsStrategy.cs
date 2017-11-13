using TSQLLint.Common;
using TSQLLint.Console.Standard.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.Standard.CommandLineOptions.CommandLineOptionStrategies
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

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            _configReader.ListPlugins();
        }
    }
}
