using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.Interface;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class RuleVisitorBuilder
    {
        private readonly ILintConfigReader ConfigReader;
        private readonly List<Type> RuleVisitors = new List<Type>()
        {
            typeof(DataCompressionOptionRule),
            typeof(DataTypeLengthRule),
            typeof(InformationSchemaRule),
            typeof(ObjectPropertyRule),
            typeof(PrintStatementRule),
            typeof(SchemaQualifyRule),
            typeof(SelectStarRule),
            typeof(SemicolonRule),
            typeof(SetAnsiNullsRule),
            typeof(SetNoCountRule),
            typeof(SetQuotedIdentifierRule),
            typeof(SetTransactionIsolationLevelRule),
            typeof(UpperLowerRule)
        };

        public RuleVisitorBuilder(ILintConfigReader configReader)
        {
            ConfigReader = configReader;
        }

        public List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<RuleViolation> violations)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();
            for (var index = 0; index < RuleVisitors.Count; index++)
            {
                var visitor = RuleVisitors[index];

                Action<string, string, TSqlFragment> ErrorCallback =
                    delegate(string ruleName, string ruleText, TSqlFragment node)
                    {
                        violations.Add(new RuleViolation(
                            sqlPath,
                            ruleName,
                            ruleText,
                            node,
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
