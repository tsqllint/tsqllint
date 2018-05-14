using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration.Overrides;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Infrastructure.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        private readonly IFragmentBuilder fragmentBuilder;

        private readonly IRuleVisitorBuilder ruleVisitorBuilder;

        private readonly IReporter reporter;

        private readonly OverrideFinder overrideFinder = new OverrideFinder();

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IFragmentBuilder fragmentBuilder, IReporter reporter)
        {
            this.fragmentBuilder = fragmentBuilder;
            this.reporter = reporter;
            this.ruleVisitorBuilder = ruleVisitorBuilder;
        }

        public void VisitRules(string sqlPath, IEnumerable<IRuleException> ignoredRules, Stream sqlFileStream)
        {
            var overrides = overrideFinder.GetOverrideList(sqlFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(GetSqlTextReader(sqlFileStream), out var errors, overrides);
            
            // notify user of syntax errors
            var ruleExceptions = ignoredRules as IRuleException[] ?? ignoredRules.ToArray();
            if (errors.Any())
            {
                HandleParserErrors(sqlPath, errors, ruleExceptions);
            }

            // lint what can be linted, even if there are errors
            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ruleExceptions);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment?.Accept(visitor);
            }
        }

        private static StreamReader GetSqlTextReader(Stream sqlFileStream)
        {
            var sqlText = new StreamReader(sqlFileStream);
            sqlFileStream.Seek(0, SeekOrigin.Begin);
            return sqlText;
        }

        private void HandleParserErrors(string sqlPath, IEnumerable<ParseError> errors, IEnumerable<IRuleException> ignoredRules)
        {
            var updatedExitCode = false;
            var ruleExceptions = ignoredRules as IRuleException[] ?? ignoredRules.ToArray();

            foreach (var error in errors)
            {
                var globalRulesOnLine = ruleExceptions.OfType<GlobalRuleException>().Where(
                    x => error.Line >= x.StartLine
                    && error.Line <= x.EndLine);

                if (!globalRulesOnLine.Any())
                {
                    reporter.ReportViolation(new RuleViolation(sqlPath, "invalid-syntax", error.Message, error.Line, error.Column, RuleViolationSeverity.Error));
                    if (updatedExitCode)
                    {
                        continue;
                    }

                    updatedExitCode = true;
                    Environment.ExitCode = 1;
                }
            }
        }
    }
}
