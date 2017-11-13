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
        private readonly IFragmentBuilder FragmentBuilder;

        private readonly IRuleVisitorBuilder RuleVisitorBuilder;

        private readonly IReporter Reporter;

        private readonly IRuleExceptionFinder RuleExceptionFinder;

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IFragmentBuilder fragmentBuilder, IReporter reporter) : this(ruleVisitorBuilder, new RuleExceptionFinder(),  fragmentBuilder, reporter) { }

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IRuleExceptionFinder ruleExceptionFinder, IFragmentBuilder fragmentBuilder, IReporter reporter)
        {
            FragmentBuilder = fragmentBuilder;
            Reporter = reporter;
            RuleExceptionFinder = ruleExceptionFinder;
            RuleVisitorBuilder = ruleVisitorBuilder;
        }

        public void VisitRules(string sqlPath, Stream sqlFileStream)
        {
            TextReader sqlTextReader = new StreamReader(sqlFileStream);
            var sqlFragment = FragmentBuilder.GetFragment(sqlTextReader, out var errors);

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
    }
}
