using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.Interface;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class RuleVisitorBuilder
    {
        private readonly IReporter Reporter;
        private readonly IConfigReader ConfigReader;
        public readonly List<Type> RuleVisitorTypes = new List<Type>()
        {
            typeof(ConditionalBeginEndRule),
            typeof(DataCompressionOptionRule),
            typeof(DataTypeLengthRule),
            typeof(DisallowCursorRule),
            typeof(InformationSchemaRule),
            typeof(KeywordCapitalizationRule),
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

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            Reporter = reporter;
            ConfigReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<RuleViolation> violations)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            for (var index = 0; index < RuleVisitorTypes.Count; index++)
            {
                var visitor = RuleVisitorTypes[index];
                Action<string, string, int, int> ErrorCallback = delegate(string ruleName, string ruleText, int startLne, int startColumn)
                {
                    Reporter.ReportViolation(new RuleViolation(
                        sqlPath,
                        ruleName,
                        ruleText,
                        startLne,
                        startColumn,
                        ConfigReader.GetRuleSeverity(ruleName)));
                };

                var visitorInstance = (ISqlRule)Activator.CreateInstance(visitor, ErrorCallback);
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