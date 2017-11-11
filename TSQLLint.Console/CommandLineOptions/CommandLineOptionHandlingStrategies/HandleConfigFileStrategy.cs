using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class HandleConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigFileFinder _configFileFinder;

        public HandleConfigFileStrategy(IBaseReporter reporter, IConfigFileFinder configFileFinder)
        {
            _reporter = reporter;
            _configFileFinder = configFileFinder;
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            commandLineOptions.ConfigFile = commandLineOptions.ConfigFile.Trim();
            var configFileExists = _configFileFinder.FindFile(commandLineOptions.ConfigFile);

            if (!configFileExists)
            {
                _reporter.Report($"Config file not found at: {commandLineOptions.ConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }
        }
    }
}
