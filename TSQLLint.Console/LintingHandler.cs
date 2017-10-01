using System.Collections.Generic;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Plugins;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Console
{
    public class LintingHandler
    {
        private readonly SqlFileProcessor Parser;

        private readonly SqlRuleVisitor RuleVisitor;

        private readonly CommandLineOptions CommandLineOptions;

        public int LintedFileCount { get; private set; }

        public IEnumerable<RuleViolation> RuleViolations { get; private set; }

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            CommandLineOptions = commandLineOptions;
            RuleViolations = new List<RuleViolation>();

            var configReader = new ConfigReader(reporter);
            configReader.LoadConfig(commandLineOptions.ConfigFile, commandLineOptions.DefaultConfigRules);

            var pluginHandler = new PluginHandler(reporter, configReader.GetPlugins());

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
