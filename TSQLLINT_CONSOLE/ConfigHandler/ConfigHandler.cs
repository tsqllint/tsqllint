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
            var configFileGenerator = new ConfigFileGenerator(reporter);
            _commandLineOptionHandler = new CommandLineOptionHandler(commandLineOptions, configFileFinder, configFileGenerator, reporter);
        }

        public bool HandleConfigs()
        {
            return _commandLineOptionHandler.HandleCommandLineOptions();
        }
    }
}