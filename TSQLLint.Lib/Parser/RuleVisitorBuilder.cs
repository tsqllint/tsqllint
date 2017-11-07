using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Rules.Interface;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
{
    public class RuleVisitorBuilder
    {
        private readonly IReporter Reporter;
        private readonly IConfigReader ConfigReader;
        private bool ErrorLogged;

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            Reporter = reporter;
            ConfigReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath)
        {
            void ErrorCallback(string ruleName, string ruleText, int startLne, int startColumn)
            {
                var ruleSeverity = ConfigReader.GetRuleSeverity(ruleName);

                if (ruleSeverity == RuleViolationSeverity.Error && !ErrorLogged)
                {
                    ErrorLogged = true;
                    Environment.ExitCode = 1;
                }

                Reporter.ReportViolation(new RuleViolation(sqlPath, ruleName, ruleText, startLne, startColumn, ruleSeverity));
            }
            
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            foreach (var visitor in RuleVisitorTypes.TypeList)
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
    }
}
