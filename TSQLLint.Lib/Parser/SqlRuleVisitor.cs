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
        private readonly TSql120Parser Parser;

        private readonly RuleVisitorBuilder RuleVisitorBuilder;

        private readonly IReporter Reporter;

        public SqlRuleVisitor(IConfigReader configReader, IReporter reporter)
        {
            Parser = new TSql120Parser(true);
            RuleVisitorBuilder = new RuleVisitorBuilder(configReader, reporter);
            Reporter = reporter;
        }

        public void VisitRules(string sqlPath, TextReader sqlTextReader)
        {
            var sqlFragment = GetFragment(sqlTextReader, out var errors);

            if (errors.Count > 0)
            {
                Reporter.ReportViolation(new RuleViolation(sqlPath, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error));
                return;
            }

            var ruleVisitors = RuleVisitorBuilder.BuildVisitors(sqlPath);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment.Accept(visitor);
            }
        }

        public void VisitRule(TextReader txtRdr, TSqlFragmentVisitor visitor)
        {
            var sqlFragment = GetFragment(txtRdr, out _);
            sqlFragment.Accept(visitor);
        }

        private TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors)
        {
            return Parser.Parse(txtRdr, out errors);
        }
    }
}
