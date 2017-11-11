using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Config;
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

        private readonly RuleExceptionFinder RuleExceptionFinder;

        public SqlRuleVisitor(IConfigReader configReader, IReporter reporter)
        {
            Parser = new TSql120Parser(true);
            RuleVisitorBuilder = new RuleVisitorBuilder(configReader, reporter);
            Reporter = reporter;
            RuleExceptionFinder = new RuleExceptionFinder();
        }

        public void VisitRules(string sqlPath, Stream sqlFileStream)
        {
            TextReader sqlTextReader = new StreamReader(sqlFileStream);
            var sqlFragment = GetFragment(sqlTextReader, out var errors);

            if (errors.Count > 0)
            {
                Reporter.ReportViolation(new RuleViolation(sqlPath, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error));
                return;
            }

            sqlFileStream.Seek(0, SeekOrigin.Begin);
            var IgnoredRules = RuleExceptionFinder.GetIgnoredRuleList(sqlFileStream).ToList();
            
            var ruleVisitors = RuleVisitorBuilder.BuildVisitors(sqlPath, IgnoredRules);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment.Accept(visitor);
            }
        }

        public void VisitRule(Stream fileStream, TSqlFragmentVisitor visitor)
        {
            var textReader = new StreamReader(fileStream);
            var sqlFragment = GetFragment(textReader, out _);
            sqlFragment.Accept(visitor);
        }

        private TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors)
        {
            return Parser.Parse(txtRdr, out errors);
        }
    }
}
