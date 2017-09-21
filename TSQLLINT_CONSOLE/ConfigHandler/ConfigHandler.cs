using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class ConfigHandler
    {
        public bool PerformLinting;

        private readonly CommandLineOptions CommandLineOptions;
        private readonly CommandLineOptionHandler CommandLineOptionHandler;
        private readonly ConfigFileGenerator ConfigFileGenerator;
        private readonly ConfigFileFinder ConfigFileFinder;
        private readonly IBaseReporter Reporter;

        public ConfigHandler(CommandLineOptions commandLineOptions, IBaseReporter reporter)
        {
            Reporter = reporter;
            CommandLineOptions = commandLineOptions;
            ConfigFileFinder = new ConfigFileFinder();
            ConfigFileGenerator = new ConfigFileGenerator(reporter);
            CommandLineOptionHandler = new CommandLineOptionHandler(CommandLineOptions, ConfigFileFinder, ConfigFileGenerator, Reporter);
        }

        public void HandleConfigs()
        {
            CommandLineOptionHandler.HandleCommandLineOptions();
            PerformLinting = CommandLineOptionHandler.PerformLinting;
        }
    }
}