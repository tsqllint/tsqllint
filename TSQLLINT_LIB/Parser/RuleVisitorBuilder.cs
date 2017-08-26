using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.Interface;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class RuleVisitorBuilder
    {
        private readonly IReporter reporter;
        private readonly IConfigReader configReader;
        private readonly List<Type> ruleVisitors = new List<Type>
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
            typeof(UpperLowerRule)
        };

        public RuleVisitorBuilder(IConfigReader configReader, IReporter reporter)
        {
            this.reporter = reporter;
            this.configReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<RuleViolation> violations)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            for (var index = 0; index < this.ruleVisitors.Count; index++)
            {
                var visitor = this.ruleVisitors[index];
                Action<string, string, int, int> errorCallback = delegate(string ruleName, string ruleText, int startLne, int startColumn)
                {
                    this.reporter.ReportViolation(new RuleViolation(
                        sqlPath,
                        ruleName,
                        ruleText,
                        startLne,
                        startColumn,
                        this.configReader.GetRuleSeverity(ruleName)));
                };

                var visitorInstance = (ISqlRule)Activator.CreateInstance(visitor, errorCallback);
                var severity = this.configReader.GetRuleSeverity(visitorInstance.RuleName);

                if (severity == RuleViolationSeverity.Error || severity == RuleViolationSeverity.Warning)
                {
                    configuredVisitors.Add((TSqlFragmentVisitor)visitorInstance);
                }
            }
            return configuredVisitors;
        }
    }
}