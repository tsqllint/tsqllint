using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class ConfigHandler
    {
        private readonly CommandLineOptions _commandLineOptions;
        private readonly CommandLineOptionHandler _commandLineOptionHandler;
        private readonly ConfigFileGenerator _configFileGenerator;
        private readonly ConfigFileFinder _configFileFinder;
        private readonly IBaseReporter _reporter;

        public ConfigHandler(CommandLineOptions commandLineOptions, IBaseReporter reporter)
        {
            _reporter = reporter;
            _commandLineOptions = commandLineOptions;
            _configFileFinder = new ConfigFileFinder();
            _configFileGenerator = new ConfigFileGenerator(reporter);
            _commandLineOptionHandler = new CommandLineOptionHandler(_commandLineOptions, _configFileFinder, _configFileGenerator, _reporter);
        }

        public bool PerformLinting { get; set; }

        public void HandleConfigs()
        {
            _commandLineOptionHandler.HandleCommandLineOptions();
            PerformLinting = _commandLineOptionHandler.PerformLinting;
        }
    }
}