using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
{
    public class PrintUsageStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;

        public PrintUsageStrategy(IBaseReporter reporter)
        {
            this.reporter = reporter;
        }

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            reporter.Report(string.Format(commandLineOptions.GetUsage()));
            return new HandlerResponseMessage(true, false);
        }
    }
}
