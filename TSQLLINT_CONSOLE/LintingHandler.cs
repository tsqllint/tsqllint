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
        public List<RuleViolation> RuleViolations = new List<RuleViolation>();

        private readonly SqlFileProcessor Parser;
        private readonly LintConfigReader ConfigReader;
        private readonly SqlRuleVisitor RuleVisitor;
        private readonly CommandLineOptionParser CommandLineOptions;

        public LintingHandler(CommandLineOptionParser commandLineOptions, IBaseReporter reporter)
        {
            CommandLineOptions = commandLineOptions;
            ConfigReader = new LintConfigReader(CommandLineOptions.ConfigFile);
            RuleVisitor = new SqlRuleVisitor(ConfigReader);
            Parser = new SqlFileProcessor(RuleVisitor, reporter);
        }

        public void Lint()
        {
            Parser.ProcessPath(CommandLineOptions.LintPath);
            RuleViolations = RuleVisitor.Violations;
            LintedFileCount = Parser.GetFileCount();
        }
    }
}