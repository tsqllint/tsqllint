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
        private readonly SqlFileProcessor _parser;
        private readonly ConfigReader _configReader;
        private readonly SqlRuleVisitor _ruleVisitor;
        private readonly CommandLineOptions _commandLineOptions;

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            _commandLineOptions = commandLineOptions;
            RuleViolations = new List<RuleViolation>();
            _configReader = new ConfigReader(_commandLineOptions.ConfigFile);
            _ruleVisitor = new SqlRuleVisitor(_configReader, reporter);
            _parser = new SqlFileProcessor(_ruleVisitor, reporter);
        }

        public int LintedFileCount { get; set; }

        public IEnumerable<RuleViolation> RuleViolations { get; set; }

        public void Lint()
        {
            _parser.ProcessList(_commandLineOptions.LintPath);
            RuleViolations = _ruleVisitor.Violations;
            LintedFileCount = _parser.GetFileCount();
        }
    }
}