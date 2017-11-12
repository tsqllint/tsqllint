using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Rules.Interface;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
{
    public class RuleVisitorBuilder : IRuleVisitorBuilder
    {
        private readonly IReporter Reporter;
        private readonly IConfigReader ConfigReader;
        private bool ErrorLogged;

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            Reporter = reporter;
            ConfigReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<IRuleException> ignoredRules)
        {
            void ErrorCallback(string ruleName, string ruleText, int startLne, int startColumn)
            {
                if (ignoredRules.Any() && IsRuleIgnored(ignoredRules, ruleName, startLne))
                {
                    return;
                }

                var ruleSeverity = ConfigReader.GetRuleSeverity(ruleName);
                if (ruleSeverity == RuleViolationSeverity.Error && !ErrorLogged)
                {
                    ErrorLogged = true;
                    Environment.ExitCode = 1;
                }

                Reporter.ReportViolation(new RuleViolation(sqlPath, ruleName, ruleText, startLne, startColumn, ruleSeverity));
            }
            
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            foreach (var visitor in RuleVisitorTypes.List)
            {
                var visitorInstance = (ISqlRule)Activator.CreateInstance(visitor, (Action<string, string, int, int>)ErrorCallback);
                var severity = ConfigReader.GetRuleSeverity(visitorInstance.RULE_NAME);

                if (severity == RuleViolationSeverity.Off)
                {
                    continue;
                }

                configuredVisitors.Add((TSqlFragmentVisitor)visitorInstance);
            }

            return configuredVisitors;
        }

        private static bool IsRuleIgnored(List<IRuleException> ignoredRules, string ruleName, int startLne)
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
    }
}
