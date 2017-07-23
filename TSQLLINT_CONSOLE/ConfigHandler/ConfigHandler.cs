using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class ConfigHandler
    {
        public bool PerformLinting;

        private readonly CommandLineOptionParser CommandLineOptions;
        private readonly CommandLineOptionHandler CommandLineOptionHandler;
        private readonly ConfigFileGenerator ConfigFileGenerator;
        private readonly ConfigFileFinder ConfigFileFinder;
        private readonly IBaseReporter Reporter;

        public ConfigHandler(CommandLineOptionParser commandLineOptions, IBaseReporter reporter)
        {
            Reporter = reporter;
            CommandLineOptions = commandLineOptions;
            CommandLineOptionHandler = new CommandLineOptionHandler();
            ConfigFileGenerator = new ConfigFileGenerator(reporter);
            ConfigFileFinder = new ConfigFileFinder();
        }

        public void HandleConfigs()
        {
            CommandLineOptionHandler.HandleCommandLineOptions(CommandLineOptions, ConfigFileFinder, ConfigFileGenerator, Reporter);
            PerformLinting = CommandLineOptionHandler.PerformLinting;
        }
    }
}