using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        public List<RuleViolation> Violations { get; set; }

        private readonly TSql120Parser Parser;

        private readonly RuleVisitorBuilder RuleVisitorBuilder;

        private readonly IReporter Reporter;

        public SqlRuleVisitor(IConfigReader configReader, IReporter reporter)
        {
            Parser = new TSql120Parser(true);
            Violations = new List<RuleViolation>();
            RuleVisitorBuilder = new RuleVisitorBuilder(configReader, reporter);
            Reporter = reporter;
        }

        public void VisitRules(string sqlPath, TextReader sqlTextReader)
        {
            IList<ParseError> errors;
            var sqlFragment = GetFragment(sqlTextReader, out errors);

            if (errors.Count > 0)
            {
                Reporter.ReportViolation(new RuleViolation(sqlPath, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error));
                return;
            }

            var ruleVisitors = RuleVisitorBuilder.BuildVisitors(sqlPath, Violations);
            for (var index = 0; index < ruleVisitors.Count; index++)
            {
                var visitor = ruleVisitors[index];
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
            return Parser.Parse(txtRdr, out errors);
        }
    }
}