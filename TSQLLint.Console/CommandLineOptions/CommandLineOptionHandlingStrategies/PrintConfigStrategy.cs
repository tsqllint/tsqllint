using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class PrintConfigStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;

        public PrintConfigStrategy(IBaseReporter reporter)
        {
            _reporter = reporter;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            _reporter.Report(!string.IsNullOrWhiteSpace(commandLineOptions.DefaultConfigRules)
                ? "Using default config instead of a file"
                : $"Config file found at: {commandLineOptions.ConfigFile}");
        }
    }
}
