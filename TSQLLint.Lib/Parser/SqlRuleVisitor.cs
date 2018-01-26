using System;
using System.IO;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Parser.RuleExceptions;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        private readonly IFragmentBuilder fragmentBuilder;

        private readonly IRuleVisitorBuilder ruleVisitorBuilder;

        private readonly IReporter reporter;

        private readonly IRuleExceptionFinder ruleExceptionFinder;

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IFragmentBuilder fragmentBuilder, IReporter reporter)
            : this(ruleVisitorBuilder, new RuleExceptionFinder(),  fragmentBuilder, reporter) { }

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IRuleExceptionFinder ruleExceptionFinder, IFragmentBuilder fragmentBuilder, IReporter reporter)
        {
            this.fragmentBuilder = fragmentBuilder;
            this.reporter = reporter;
            this.ruleExceptionFinder = ruleExceptionFinder;
            this.ruleVisitorBuilder = ruleVisitorBuilder;
        }

        public void VisitRules(string sqlPath, Stream sqlFileStream)
        {
            TextReader sqlTextReader = new StreamReader(sqlFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(sqlTextReader, out var errors);

            if (sqlFragment == null)
            {
                 reporter.ReportViolation(new RuleViolation(sqlPath, null, "TSQL file not parseable", 0, 0, RuleViolationSeverity.Error));
                 Environment.ExitCode = 1;
                 return;
            }

            sqlFileStream.Seek(0, SeekOrigin.Begin);
            var ignoredRules = ruleExceptionFinder.GetIgnoredRuleList(sqlFileStream).ToList();

            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ignoredRules);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment.Accept(visitor);
            }
        }
    }
}
