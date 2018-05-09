using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class SqlRuleVisitorTests
    {
        private const string Path = @"c:\testFile.sql";

        [Test]
        public void VisitRules_InvalidSql_ShouldStillLint()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            var fragmentBuilder = new FragmentBuilder(120);

            var ruleViolations = new List<RuleViolation>();
            void ErrorCallback(string ruleName, string ruleText, int startLine, int startColumn)
            {
                ruleViolations.Add(new RuleViolation(ruleName, startLine, startColumn));
            }

            var visitors = new List<TSqlFragmentVisitor>
            {
                new KeywordCapitalizationRule(ErrorCallback)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);
            var sqlStream = ParsingUtility.GenerateStreamFromString("select");
            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, fragmentBuilder, mockReporter);

            // act
            sqlRuleVisitor.VisitRules(Path, new List<IRuleException>(), sqlStream);

            // assert
            Assert.AreEqual(1, ruleViolations.Count);
        }

        [Test]
        public void VisitRules_ValidSql_ShouldVisitRules()
        {
            // arrange
            var sqlStream = ParsingUtility.GenerateStreamFromString("SELECT 1;");

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
            mockRuleExceptionFinder.GetIgnoredRuleList(Arg.Any<Stream>()).Returns(new List<IExtendedRuleException>());

            var visitors = new List<TSqlFragmentVisitor>
            {
                new SemicolonTerminationRule(null)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);

            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, mockFragmentBuilder, mockReporter);

            // act
            sqlRuleVisitor.VisitRules(Path, new List<IRuleException>(), sqlStream);

            // assert
            mockFragment.Received().Accept(Arg.Any<TSqlFragmentVisitor>());
        }

        [Test]
        public void VisitRules_GlobalIgnoreNoFragments_ShouldNotReportErrors()
        {
            // arrange
            var sqlStream = ParsingUtility.GenerateStreamFromString(@"/* tsqllint-disable */
@@Scripts\dfu_setutp_import_cleanup.sql");

            var mockReporter = Substitute.For<IReporter>();
            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();

            var visitors = new List<TSqlFragmentVisitor>
            {
                new SetAnsiNullsRule(null)
            };

            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);

            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, new FragmentBuilder(120), mockReporter);

            // act
            sqlRuleVisitor.VisitRules(Path, new List<IRuleException> { new GlobalRuleException(0, 99) }, sqlStream);

            // assert
            mockReporter.DidNotReceive().ReportViolation(Arg.Any<IRuleViolation>());
        }

        [Test]
        public void VisitRules_NullFragent_ShouldReportErrors()
        {
            // arrange
            var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"UnitTests/Parser/invalid-characters.sql"));
            var fileStream = File.OpenRead(path);

            var mockReporter = Substitute.For<IReporter>();
            var visitors = new List<TSqlFragmentVisitor>
            {
                new SetAnsiNullsRule(null)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);
            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, new FragmentBuilder(120), mockReporter);

            // act
            sqlRuleVisitor.VisitRules(Path, new List<IRuleException>(), fileStream);

            // assert
            mockReporter.Received().ReportViolation(Arg.Any<IRuleViolation>());
        }

        [Test]
        public void VisitRules_NullFragentWithIgnores_ShouldNotReportErrors()
        {
            // arrange
            var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"UnitTests/Parser/invalid-characters-ignore-rules.sql"));
            var fileStream = File.OpenRead(path);

            var mockReporter = Substitute.For<IReporter>();
            var visitors = new List<TSqlFragmentVisitor>
            {
                new SetAnsiNullsRule(null)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<List<IRuleException>>()).Returns(visitors);

            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, new FragmentBuilder(120), mockReporter);

            // act
            sqlRuleVisitor.VisitRules(Path, new List<IRuleException> { new GlobalRuleException(0, 99) }, fileStream);

            // assert
            mockReporter.DidNotReceive().ReportViolation(Arg.Any<IRuleViolation>());
        }
    }
}
