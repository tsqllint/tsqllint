using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
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
        private bool errorLogged;

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            this.reporter = reporter;
            this.configReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, IEnumerable<IRuleException> ignoredRules)
        {
            void RuleViolationCallback(string ruleName, string ruleText, int startLne, int startColumn)
            {
                HandleRuleViolation(sqlPath, ignoredRules, ruleName, startLne, ruleText, startColumn);
            }

            var configuredVisitors = new List<TSqlFragmentVisitor>();

            foreach (var visitor in RuleVisitorFriendlyNameTypeMap.List)
            {
                var visitorInstance = (ISqlRule)Activator.CreateInstance(
                    visitor.Value,
                    (Action<string, string, int, int>)RuleViolationCallback);

                var severity = configReader.GetRuleSeverity(visitorInstance.RULE_NAME);
                if (severity == RuleViolationSeverity.Off)
                {
                    continue;
                }

                configuredVisitors.Add((TSqlFragmentVisitor)visitorInstance);
            }

            return configuredVisitors;
        }

        private static bool IsRuleIgnored(IEnumerable<IRuleException> ignoredRules, string ruleName, int startLne)
        {
            var friendlyNameToType = RuleVisitorFriendlyNameTypeMap.List.Where(x => x.Key == ruleName);
            var ruleType = friendlyNameToType.FirstOrDefault().Value;

            var rulesOnLine = ignoredRules.OfType<RuleException>().Where(
                x => x.RuleType.Name == ruleType.Name
                     && startLne >= x.StartLine
                     && startLne <= x.EndLine);

            var globalRulesOnLine = ignoredRules.OfType<GlobalRuleException>().Where(
                x => startLne >= x.StartLine
                     && startLne <= x.EndLine);

            return rulesOnLine.Any() || globalRulesOnLine.Any();
        }

        private void HandleRuleViolation(string sqlPath, IEnumerable<IRuleException> ignoredRules, string ruleName, int startLne, string ruleText, int startColumn)
        {
            if (ignoredRules.Any() && IsRuleIgnored(ignoredRules, ruleName, startLne))
            {
                return;
            }

            var ruleSeverity = configReader.GetRuleSeverity(ruleName);
            if (ruleSeverity == RuleViolationSeverity.Error && !errorLogged)
            {
                errorLogged = true;
                Environment.ExitCode = 1;
            }

            reporter.ReportViolation(new RuleViolation(sqlPath, ruleName, ruleText, startLne, startColumn, ruleSeverity));
        }
    }
}
