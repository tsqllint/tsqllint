using System.Collections.Generic;
using TSQLLINT_COMMON;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Plugins;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_CONSOLE
{
    public class LintingHandler
    {
        public int LintedFileCount;
        public IEnumerable<RuleViolation> RuleViolations = new List<RuleViolation>();

        private readonly SqlFileProcessor Parser;
        private readonly ConfigReader ConfigReader;
        private readonly SqlRuleVisitor RuleVisitor;
        private readonly CommandLineOptions CommandLineOptions;
        private readonly PluginHandler PluginHandler;

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            CommandLineOptions = commandLineOptions;
            ConfigReader = new ConfigReader(reporter, CommandLineOptions.ConfigFile);
            PluginHandler = new PluginHandler(reporter, ConfigReader.GetPlugins());
            RuleVisitor = new SqlRuleVisitor(ConfigReader, reporter);
            Parser = new SqlFileProcessor(PluginHandler, RuleVisitor, reporter);
        }

        public void Lint()
        {
            Parser.ProcessList(CommandLineOptions.LintPath);
            RuleViolations = RuleVisitor.Violations;
            LintedFileCount = Parser.GetFileCount();
        }
    }
}