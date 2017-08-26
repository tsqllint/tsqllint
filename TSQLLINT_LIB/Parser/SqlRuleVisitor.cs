using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        private readonly TSql120Parser parser;
        private readonly RuleVisitorBuilder ruleVisitorBuilder;
        private readonly IReporter reporter;

        public SqlRuleVisitor(IConfigReader configReader, IReporter reporter)
        {
            this.parser = new TSql120Parser(true);
            Violations = new List<RuleViolation>();
            this.ruleVisitorBuilder = new RuleVisitorBuilder(configReader, reporter);
            this.reporter = reporter;
        }

        public List<RuleViolation> Violations { get; set; }

        public void VisitRules(string sqlPath, TextReader sqlTextReader)
        {
            IList<ParseError> errors;
            var sqlFragment = GetFragment(sqlTextReader, out errors);

            if (errors.Count > 0)
            {
                this.reporter.ReportViolation(new RuleViolation(sqlPath, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error));
                return;
            }

            for (var index = 0; index < this.ruleVisitorBuilder.BuildVisitors(sqlPath, Violations).Count; index++)
            {
                var visitor = this.ruleVisitorBuilder.BuildVisitors(sqlPath, Violations)[index];
                sqlFragment.Accept(visitor);
            }
        }

        public void VisitRule(TextReader txtRdr, TSqlFragmentVisitor visitor)
        {
            IList<ParseError> errors;
            var sqlFragment = GetFragment(txtRdr, out errors);
            sqlFragment.Accept(visitor);
        }

        private TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors)
        {
            return this.parser.Parse(txtRdr, out errors);
        }
    }
}