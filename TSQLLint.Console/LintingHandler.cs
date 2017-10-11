using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Plugins;

namespace TSQLLint.Console
{
    public class LintingHandler
    {
        private readonly SqlFileProcessor Parser;

        private readonly SqlRuleVisitor RuleVisitor;

        private readonly CommandLineOptions CommandLineOptions;

        public int LintedFileCount { get; private set; }

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            CommandLineOptions = commandLineOptions;

            var configReader = new ConfigReader(reporter);
            configReader.LoadConfig(commandLineOptions.ConfigFile, commandLineOptions.DefaultConfigRules);

            var pluginHandler = new PluginHandler(reporter, configReader.GetPlugins());

            RuleVisitor = new SqlRuleVisitor(configReader, reporter);
            Parser = new SqlFileProcessor(pluginHandler, RuleVisitor, reporter);
        }

        public void Lint()
        {
            Parser.ProcessList(CommandLineOptions.LintPath);
            LintedFileCount = Parser.GetFileCount();
        }
    }
}
