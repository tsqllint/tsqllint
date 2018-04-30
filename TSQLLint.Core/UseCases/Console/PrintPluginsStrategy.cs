using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console
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

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            configReader.ListPlugins();
        }
    }
}
