using System.Collections.Generic;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
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

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            CommandLineOptions = commandLineOptions;
            ConfigReader = new ConfigReader(CommandLineOptions.ConfigFile);
            RuleVisitor = new SqlRuleVisitor(ConfigReader, reporter);
            Parser = new SqlFileProcessor(RuleVisitor, reporter);
        }

        public void Lint()
        {
            Parser.ProcessList(CommandLineOptions.LintPath);
            RuleViolations = RuleVisitor.Violations;
            LintedFileCount = Parser.GetFileCount();
        }
    }
}