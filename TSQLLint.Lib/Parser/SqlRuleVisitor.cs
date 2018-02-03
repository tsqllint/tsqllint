using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
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
                HandleParserErrors(sqlPath, errors);
            }

            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ignoredRules);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment?.Accept(visitor);
            }
        }

        private void HandleParserErrors(string sqlPath, IList<Microsoft.SqlServer.TransactSql.ScriptDom.ParseError> errors)
        {
            foreach (var error in errors)
            {
                reporter.ReportViolation(new RuleViolation(sqlPath, "invalid-syntax", error.Message, error.Line, error.Column, RuleViolationSeverity.Error));
            }

            Environment.ExitCode = 1;
        }
    }
}
