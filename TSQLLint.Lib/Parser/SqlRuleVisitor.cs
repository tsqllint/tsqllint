using System.Collections.Generic;
using System.IO;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;

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

            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ignoredRules);
            foreach (var visitor in ruleVisitors)
            {
                sqlFragment.Accept(visitor);
            }
        }
    }
}
