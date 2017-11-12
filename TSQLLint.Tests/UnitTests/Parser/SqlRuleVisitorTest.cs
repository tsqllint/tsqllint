using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class SqlRuleVisitorTests
    {
        private const string path = @"c:\testFile.sql";
        
        [Test]
        public void VisitRules_InvalidSql_ShouldReportError()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            var fragmentBuilder = new FragmentBuilder();
            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            var sqlStream = ParsingUtility.GenerateStreamFromString("SELECT");
            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, fragmentBuilder, mockReporter);

            // act
            sqlRuleVisitor.VisitRules(path, sqlStream);

            // assert
            mockReporter.Received().ReportViolation(Arg.Is<RuleViolation>(x => x.FileName == path && x.Text == "TSQL not syntactically correct"));
        }

        [Test]
        public void VisitRules_ValidSql_ShouldVisitRules()
        {
            // arrange
            var sqlStream = ParsingUtility.GenerateStreamFromString("SELECT 1");

            var mockReporter = Substitute.For<IReporter>();
            var mockFragment = Substitute.For<TSqlFragment>();

            var mockFragmentBuilder = Substitute.For<IFragmentBuilder>();
            IList<ParseError> mockErrors = new List<ParseError>();
            mockFragmentBuilder.GetFragment(Arg.Any<TextReader>(), out var errors).Returns(x =>
            {
                x[1] = mockErrors;
                return mockFragment;
            });

            var mockRuleExceptionFinder = Substitute.For<IRuleExceptionFinder>();
            mockRuleExceptionFinder.GetIgnoredRuleList(Arg.Any<Stream>()).Returns(new List<IRuleException>());
            
            var visitors = new List<TSqlFragmentVisitor>
            {
                new SemicolonTerminationRule(null)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);
            
            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, mockRuleExceptionFinder, mockFragmentBuilder, mockReporter);

            // act
            sqlRuleVisitor.VisitRules(path, sqlStream);

            // assert
            mockFragment.Received().Accept(Arg.Any<TSqlFragmentVisitor>());
        }
    }
}
