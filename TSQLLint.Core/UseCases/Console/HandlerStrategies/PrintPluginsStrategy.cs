using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
{
    public class PrintPluginsStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IConfigReader configReader;

        public PrintPluginsStrategy(IBaseReporter reporter, IConfigReader configReader)
        {
            this.reporter = reporter;
            this.configReader = configReader;
        }

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            configReader.ListPlugins();
            return new HandlerResponseMessage(true, false);
        }
    }
}
