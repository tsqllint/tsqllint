using TSQLLint.Common;
using TSQLLint.Console.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class PrintConfigStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IConfigReader configReader;

        public PrintConfigStrategy(IBaseReporter reporter, IConfigReader configReader)
        {
            this.reporter = reporter;
            this.configReader = configReader;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            reporter.Report(string.IsNullOrEmpty(configReader.ConfigFileLoadedFrom)
                    ? "Using default in memory config." : $"Config file found at: {configReader.ConfigFileLoadedFrom}");
        }
    }
}
