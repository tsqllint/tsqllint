using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Console.Standard.Interfaces;

namespace TSQLLint.Console.Standard.CommandLineOptions.CommandLineOptionStrategies
{
    public class LoadConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IFileSystem _fileSystem;

        public LoadConfigFileStrategy(IBaseReporter reporter) : this(reporter, new FileSystem()) { }

        public LoadConfigFileStrategy(IBaseReporter reporter, FileSystem fileSystem)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions)
        {
            commandLineOptions.ConfigFile = commandLineOptions.ConfigFile.Trim();
            if (!_fileSystem.File.Exists(commandLineOptions.ConfigFile))
            {
                _reporter.Report($"Config file not found at: {commandLineOptions.ConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }

            if (!commandLineOptions.LintPath.Any())
            {
                _reporter.Report(commandLineOptions.GetUsage());
            }
        }
    }
}
