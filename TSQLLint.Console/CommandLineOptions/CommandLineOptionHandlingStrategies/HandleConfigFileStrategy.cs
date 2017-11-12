using System;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class HandleConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IFileSystem _fileSystem;

        public HandleConfigFileStrategy(IBaseReporter reporter) : this(reporter, new FileSystem()) { }

        public HandleConfigFileStrategy(IBaseReporter reporter, FileSystem fileSystem)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
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
