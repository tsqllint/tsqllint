using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.Interface;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
{
    public class RuleVisitorBuilder
    {
        private readonly IReporter Reporter;
        private readonly IConfigReader ConfigReader;
        public readonly List<Type> RuleVisitorTypes = new List<Type>
        {
            typeof(ConditionalBeginEndRule),
            typeof(CrossDatabaseRule),
            typeof(DataCompressionOptionRule),
            typeof(DataTypeLengthRule),
            typeof(DisallowCursorRule),
            typeof(InformationSchemaRule),
            typeof(KeywordCapitalizationRule),
            typeof(LinkedServerRule),
            typeof(MultiTableAliasRule),
            typeof(ObjectPropertyRule),
            typeof(PrintStatementRule),
            typeof(SchemaQualifyRule),
            typeof(SelectStarRule),
            typeof(SemicolonTerminationRule),
            typeof(SetAnsiNullsRule),
            typeof(SetNoCountRule),
            typeof(SetQuotedIdentifierRule),
            typeof(SetTransactionIsolationLevelRule),
            typeof(SetVariableRule),
            typeof(UpperLowerRule)
        };

        private bool ErrorLogged;

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            Reporter = reporter;
            ConfigReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            foreach (var visitor in RuleVisitorTypes)
            {
                void ErrorCallback(string ruleName, string ruleText, int startLne, int startColumn)
                {
                    var ruleSeverity = ConfigReader.GetRuleSeverity(ruleName);

                    if (!ErrorLogged && ruleSeverity == RuleViolationSeverity.Error)
                    {
                        ErrorLogged = true;
                        Environment.ExitCode = 1;
                    }

                    Reporter.ReportViolation(new RuleViolation(sqlPath, ruleName, ruleText, startLne, startColumn, ruleSeverity));
                }

                var visitorInstance = (ISqlRule)Activator.CreateInstance(visitor, (Action<string, string, int, int>)ErrorCallback);
                var severity = ConfigReader.GetRuleSeverity(visitorInstance.RULE_NAME);

                if (severity == RuleViolationSeverity.Error || severity == RuleViolationSeverity.Warning)
                {
                    configuredVisitors.Add((TSqlFragmentVisitor)visitorInstance);
                }
            }

            return configuredVisitors;
        }
    }
}
