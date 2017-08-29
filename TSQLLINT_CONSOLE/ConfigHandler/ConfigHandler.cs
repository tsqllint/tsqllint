using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class ConfigHandler
    {
        private readonly CommandLineOptionHandler _commandLineOptionHandler;

        public ConfigHandler(CommandLineOptions commandLineOptions, IBaseReporter reporter)
        {
            var configFileFinder = new ConfigFileFinder();
            commandLineOptions.ConfigFileFinder = configFileFinder;
            var configFileGenerator = new ConfigFileGenerator(reporter);
            _commandLineOptionHandler = new CommandLineOptionHandler(commandLineOptions, configFileFinder, configFileGenerator, reporter);
        }

        public bool PerformLinting { get; set; }

        public void HandleConfigs()
        {
            _commandLineOptionHandler.HandleCommandLineOptions();
            PerformLinting = _commandLineOptionHandler.PerformLinting;
        }
    }
}