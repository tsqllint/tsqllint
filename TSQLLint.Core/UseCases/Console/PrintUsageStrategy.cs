using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console
{
    public class PrintUsageStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;

        public PrintUsageStrategy(IBaseReporter reporter)
        {
            this.reporter = reporter;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            reporter.Report(string.Format(commandLineOptions.GetUsage()));
        }
    }
}