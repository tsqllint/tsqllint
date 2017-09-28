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
        private readonly SqlRuleVisitor RuleVisitor;
        private readonly CommandLineOptions CommandLineOptions;

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            CommandLineOptions = commandLineOptions;
            RuleViolations = new List<RuleViolation>();

            var configReader = new ConfigReader(reporter);
            var pluginHandler = new PluginHandler(reporter, configReader.GetPlugins());

            if (!string.IsNullOrWhiteSpace(commandLineOptions.DefaultConfigRules))
            {
                configReader.LoadConfigFromRules(commandLineOptions.DefaultConfigRules);
            }
            else
            {
                configReader.LoadConfigFromFile(commandLineOptions.ConfigFile);
            }
            
            RuleVisitor = new SqlRuleVisitor(configReader, reporter);
            Parser = new SqlFileProcessor(pluginHandler, RuleVisitor, reporter);
        }

        public void Lint()
        {
            Parser.ProcessList(CommandLineOptions.LintPath);
            RuleViolations = RuleVisitor.Violations;
            LintedFileCount = Parser.GetFileCount();
        }
    }
}
