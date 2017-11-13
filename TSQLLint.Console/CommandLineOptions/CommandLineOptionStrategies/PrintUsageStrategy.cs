using TSQLLint.Common;
using TSQLLint.Console.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class PrintUsageStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        
        public PrintUsageStrategy(IBaseReporter reporter)
        {
            _reporter = reporter;
        }
        
        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            _reporter.Report(string.Format(commandLineOptions.GetUsage()));
        }
    }
}
