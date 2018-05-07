using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
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

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IFragmentBuilder fragmentBuilder, IReporter reporter)
        {
            this.fragmentBuilder = fragmentBuilder;
            this.reporter = reporter;
            this.ruleVisitorBuilder = ruleVisitorBuilder;
        }

        public void VisitRules(string sqlPath, IEnumerable<IRuleException> ignoredRules, Stream sqlFileStream)
        {
            TextReader sqlTextReader = new StreamReader(sqlFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(sqlTextReader, out var errors);
            sqlFileStream.Seek(0, SeekOrigin.Begin);

            if (errors.Any())
            {
                HandleParserErrors(sqlPath, errors, ignoredRules);
            }

            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ignoredRules);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment?.Accept(visitor);
            }
        }

        private void HandleParserErrors(string sqlPath, IEnumerable<ParseError> errors, IEnumerable<IRuleException> ignoredRules)
        {
            var updatedExitCode = false;

            foreach (var error in errors)
            {
                var globalRulesOnLine = ignoredRules.OfType<GlobalRuleException>().Where(
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
