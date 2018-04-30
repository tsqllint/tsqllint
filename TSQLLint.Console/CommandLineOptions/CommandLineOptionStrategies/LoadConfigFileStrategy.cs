using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class LoadConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;
        private readonly IFileSystem fileSystem;

        public LoadConfigFileStrategy(IBaseReporter reporter)
            : this(reporter, new FileSystem()) { }

        public LoadConfigFileStrategy(IBaseReporter reporter, FileSystem fileSystem)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            commandLineOptions.ConfigFile = commandLineOptions.ConfigFile.Trim();
            if (!fileSystem.File.Exists(commandLineOptions.ConfigFile))
            {
                reporter.Report($"Config file not found at: {commandLineOptions.ConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }

            if (!commandLineOptions.LintPath.Any())
            {
                reporter.Report(commandLineOptions.GetUsage());
            }
        }
    }
}
