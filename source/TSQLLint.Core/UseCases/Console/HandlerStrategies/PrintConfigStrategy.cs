using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
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

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            reporter.Report(string.IsNullOrEmpty(configReader.ConfigFileLoadedFrom)
                    ? "Using default in memory config." : $"Config file found at: {configReader.ConfigFileLoadedFrom}");
            return new HandlerResponseMessage(true, false);
        }
    }
}
