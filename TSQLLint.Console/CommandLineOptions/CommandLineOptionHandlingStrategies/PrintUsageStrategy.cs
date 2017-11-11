using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class PrintUsageStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        
        public PrintUsageStrategy(IBaseReporter reporter)
        {
            _reporter = reporter;
        }
        
        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            _reporter.Report(string.Format(commandLineOptions.GetUsage()));
        }
    }
}
