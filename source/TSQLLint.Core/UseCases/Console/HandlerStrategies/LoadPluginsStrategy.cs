using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
{
    internal class LoadPluginsStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IFileSystemWrapper fileSystem;

        public LoadPluginsStrategy(IBaseReporter reporter, IFileSystemWrapper fileSystem)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
        }

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            if (string.IsNullOrEmpty(commandLineOptions.Plugins.Trim()))
            {
                reporter.Report($"No plugins specified to be loaded");
            }

            return new HandlerResponseMessage(true, true, false, true);
        }
    }
}
