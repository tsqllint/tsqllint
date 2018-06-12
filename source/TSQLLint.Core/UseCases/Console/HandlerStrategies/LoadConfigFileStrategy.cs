using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
{
    public class LoadConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IFileSystemWrapper fileSystem;

        public LoadConfigFileStrategy(IBaseReporter reporter, IFileSystemWrapper fileSystem)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
        }

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            commandLineOptions.ConfigFile = commandLineOptions.ConfigFile.Trim();
            if (!fileSystem.FileExists(commandLineOptions.ConfigFile))
            {
                reporter.Report($"Config file not found at: {commandLineOptions.ConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }

            if (!commandLineOptions.LintPath.Any())
            {
                reporter.Report(commandLineOptions.GetUsage());
                return new HandlerResponseMessage(true, false);
            }

            return new HandlerResponseMessage(true, true);
        }
    }
}
