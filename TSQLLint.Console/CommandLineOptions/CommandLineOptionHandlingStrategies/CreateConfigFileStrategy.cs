using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class CreateConfigFileStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IConfigFileFinder _configFileFinder;

        public CreateConfigFileStrategy(IBaseReporter reporter, IConfigFileFinder configFileFinder, IConfigFileGenerator configFileGenerator)
        {
            _reporter = reporter;
            _configFileFinder = configFileFinder;
            _configFileGenerator = configFileGenerator;
        }

        private void CreateConfigFile()
        {
            _configFileGenerator.WriteConfigFile(_configFileFinder.DefaultConfigFileName);
        }

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions)
        {
            var configFileExists = _configFileFinder.FindFile(_configFileFinder.DefaultConfigFileName);
            
            if (!configFileExists)
            {
                CreateConfigFile();
            }
            else if (commandLineOptions.Force)
            {
                CreateConfigFile();
            }
            else
            {
                _reporter.Report($"Config file not found at: {_configFileFinder.DefaultConfigFileName} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }
        }
    }
}
