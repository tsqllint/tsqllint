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
        private readonly SqlRuleVisitor _ruleVisitor;
        private readonly CommandLineOptions _commandLineOptions;

        public int LintedFileCount { get; set; }
        public IEnumerable<RuleViolation> RuleViolations { get; set; }

        public LintingHandler(CommandLineOptions commandLineOptions, IReporter reporter)
        {
            RuleViolations = new List<RuleViolation>();;
            _commandLineOptions = commandLineOptions;
            var configReader = new ConfigReader();
            if (!string.IsNullOrWhiteSpace(_commandLineOptions.DefaultConfigRules))
            {
                configReader.LoadConfigFromRules(_commandLineOptions.DefaultConfigRules);
            }
            else
            {
                configReader.LoadConfigFromFile(_commandLineOptions.ConfigFile);
            }
            _ruleVisitor = new SqlRuleVisitor(configReader, reporter);
            _parser = new SqlFileProcessor(_ruleVisitor, reporter);
        }

        public void Lint()
        {
            _parser.ProcessList(_commandLineOptions.LintPath);
            RuleViolations = _ruleVisitor.Violations;
            LintedFileCount = _parser.GetFileCount();
        }
    }
}