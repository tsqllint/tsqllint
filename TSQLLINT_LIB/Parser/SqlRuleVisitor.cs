using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        public List<RuleViolation> Violations { get; set; }
        private TSql120Parser Parser;
        private List<Type> RuleVisitors;

        /// <summary>
        /// Configures the parser without rules
        /// </summary>
        public SqlRuleVisitor()
        {
            Parser = new TSql120Parser(true);
            Violations = new List<RuleViolation>();

            // get all classes that implement the ISqlRule interface
            var type = typeof(ISqlRule);
            RuleVisitors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToList();
        }

        public void VisitRules(ILintConfigReader configReader, string sqlPath, TextReader sqlTextReader)
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
                            configReader.GetRuleSeverity(ruleName)));
                    };

                var visitorInstance = (ISqlRule) Activator.CreateInstance(visitor, ErrorCallback);
                var severity = configReader.GetRuleSeverity(visitorInstance.RULE_NAME);

                if (severity == RuleViolationSeverity.Error || severity == RuleViolationSeverity.Warning)
                {
                    configuredVisitors.Add((TSqlFragmentVisitor) visitorInstance);
                }
            }

            if (configuredVisitors.Count < 1)
            {
                throw new Exception("No rule visitors are configured");
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

