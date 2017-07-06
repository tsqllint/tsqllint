using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        public List<RuleViolation> Violations { get; set; }
        private readonly TSql120Parser Parser;
        private readonly ILintConfigReader ConfigReader;

        private readonly List<Type> RuleVisitors = new List<Type>()
        {
            typeof(DataCompressionOptionRule),
            typeof(DataTypeLengthRule),
            typeof(InformationSchemaRule),
            typeof(ObjectPropertyRule),
            typeof(SchemaQualifyRule),
            typeof(SelectStarRule),
            typeof(SemicolonRule),
            typeof(SetNoCountRule),
            typeof(SetTransactionIsolationLevelRule)
        };

        /// <summary>
        /// Configures the parser without rules
        /// </summary>
        public SqlRuleVisitor(ILintConfigReader configReader)
        {
            ConfigReader = configReader;
            Parser = new TSql120Parser(true);
            Violations = new List<RuleViolation>();
        }

        public void VisitRules(string sqlPath, TextReader sqlTextReader)
        {
            var configuredVisitors = new List<TSqlFragmentVisitor>();

            for (var index = 0; index < RuleVisitors.Count; index++)
            {
                var visitor = RuleVisitors[index];

                Action<string, string, TSqlFragment> ErrorCallback =
                    delegate(string ruleName, string ruleText, TSqlFragment node)
                    {
                        Violations.Add(new RuleViolation(
                            sqlPath,
                            ruleName,
                            ruleText,
                            node,
                            ConfigReader.GetRuleSeverity(ruleName)));
                    };

                var visitorInstance = (ISqlRule) Activator.CreateInstance(visitor, ErrorCallback);
                var severity = ConfigReader.GetRuleSeverity(visitorInstance.RULE_NAME);

                if (severity == RuleViolationSeverity.Error || severity == RuleViolationSeverity.Warning)
                {
                    configuredVisitors.Add((TSqlFragmentVisitor) visitorInstance);
                }
            }

            var sqlFragment = GetFragment(sqlTextReader);
            foreach (var visitor in configuredVisitors)
            {
                sqlFragment.Accept(visitor);
            }
        }

        public void VisistRule(TextReader txtRdr, TSqlFragmentVisitor visitor)
        {
            var sqlFragment = GetFragment(txtRdr);
            sqlFragment.Accept(visitor);
        }

        private TSqlFragment GetFragment(TextReader txtRdr)
        {
            IList<ParseError> errors;
            var fragment = Parser.Parse(txtRdr, out errors);

            if (errors.Count > 0)
            {
                throw new Exception(String.Format("Errors found while parsing file: {0}", errors.FirstOrDefault().Message));
            }

            return fragment;
        }
    }
}

