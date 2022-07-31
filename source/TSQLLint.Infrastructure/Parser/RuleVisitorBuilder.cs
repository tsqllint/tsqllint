using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Infrastructure.Parser
{
    public class RuleVisitorBuilder : IRuleVisitorBuilder
    {
        private readonly IReporter reporter;
        private readonly IConfigReader configReader;
        private readonly IDictionary<string, ISqlLintRule> rules;
        private bool errorLogged;

        public RuleVisitorBuilder(
            IConfigReader configReader,
            IReporter reporter,
            IDictionary<string, ISqlLintRule> rules)
        {
            this.reporter = reporter;
            this.configReader = configReader;
            this.rules = rules;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, IEnumerable<IRuleException> ignoredRules)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();

            foreach (var visitor in rules)
            {
                var severity = configReader.GetRuleSeverity(visitor.Value.RULE_NAME, visitor.Value.RULE_SEVERITY);

                void RuleViolationCallback(string ruleName, string ruleText, int startLne, int startColumn)
                {
                    HandleRuleViolation(sqlPath, ignoredRules, ruleName, startLne, ruleText, startColumn, severity);
                }

                var visitorInstance = (ISqlLintRule)Activator.CreateInstance(
                    visitor.Value.GetType(),
                    (Action<string, string, int, int>)RuleViolationCallback);

                if (severity == RuleViolationSeverity.Off)
                {
                    continue;
                }

                configuredVisitors.Add((TSqlFragmentVisitor)visitorInstance);
            }

            return configuredVisitors;
        }

        private bool IsRuleIgnored(IEnumerable<IRuleException> ignoredRules, string ruleName, int startLne)
        {
            var friendlyNameToType = rules.Where(x => x.Key == ruleName);
            var ruleType = friendlyNameToType.FirstOrDefault().Value.GetType();

            var rulesOnLine = ignoredRules.OfType<RuleException>().Where(
                x => x.RuleType.Name == ruleType.Name
                     && startLne >= x.StartLine
                     && startLne <= x.EndLine);

            var globalRulesOnLine = ignoredRules.OfType<GlobalRuleException>().Where(
                x => startLne >= x.StartLine
                     && startLne <= x.EndLine);

            return rulesOnLine.Any() || globalRulesOnLine.Any();
        }

        private void HandleRuleViolation(string sqlPath, IEnumerable<IRuleException> ignoredRules, string ruleName, int startLne, string ruleText, int startColumn, RuleViolationSeverity ruleSeverity)
        {
            if (ignoredRules.Any() && IsRuleIgnored(ignoredRules, ruleName, startLne))
            {
                return;
            }

            if (ruleSeverity == RuleViolationSeverity.Error && !errorLogged)
            {
                errorLogged = true;
                Environment.ExitCode = 1;
            }

            var ruleViolation = new RuleViolation(sqlPath, ruleName, ruleText, startLne, startColumn, ruleSeverity);

            reporter.ReportViolation(ruleViolation);
        }
    }
}
