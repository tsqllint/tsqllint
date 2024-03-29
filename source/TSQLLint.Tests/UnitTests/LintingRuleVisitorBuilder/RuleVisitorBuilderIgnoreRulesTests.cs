using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleExceptions;

namespace TSQLLint.Tests.UnitTests.LintingRuleVisitorBuilder
{
    [TestFixture]
    public class RuleVisitorBuilderIgnoreRulesTests
    {
        private readonly IFragmentBuilder fragmentBuilder = new FragmentBuilder(120);

        [Test]
        public void RuleVisitorEnforcesRule()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            mockReporter.ReportViolation(Arg.Any<IRuleViolation>());

            var mockConfigReader = Substitute.For<IConfigReader>();
            mockConfigReader.GetRuleSeverity("select-star").Returns(RuleViolationSeverity.Error);

            var ignoredRuleList = new List<IExtendedRuleException>();

            var pathString = "DoesntExist.sql";
            var ruleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter, RuleVisitorFriendlyNameTypeMap.Rules);
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO;");
            var textReader = new StreamReader(testFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);

            // act
            foreach (var sqlFragmentVisitor in activeRuleVisitors)
            {
                sqlFragment.Accept(sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(1, activeRuleVisitors.Count);
            Assert.IsTrue(activeRuleVisitors[0].GetType().Name == typeof(SelectStarRule).Name);
            mockReporter.Received().ReportViolation(Arg.Is<IRuleViolation>(x =>
                x.FileName == pathString
                && x.RuleName == "select-star"
                && x.Line == 1
                && x.Severity == RuleViolationSeverity.Error));
        }

        [Test]
        public void RuleVisitorIgnoresRule()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            mockReporter.ReportViolation(Arg.Any<IRuleViolation>());

            var mockConfigReader = Substitute.For<IConfigReader>();
            mockConfigReader.GetRuleSeverity("select-star").Returns(RuleViolationSeverity.Error);

            var ignoredRuleList = new List<IExtendedRuleException>
            {
                new RuleException(typeof(SelectStarRule), "select-star", 1, 10)
            };

            var pathString = "DoesntExist.sql";
            var ruleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter, RuleVisitorFriendlyNameTypeMap.Rules);
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO;");
            var textReader = new StreamReader(testFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);

            // act
            foreach (var sqlFragmentVisitor in activeRuleVisitors)
            {
                sqlFragment.Accept(sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(1, activeRuleVisitors.Count);
            Assert.IsTrue(activeRuleVisitors[0].GetType().Name == typeof(SelectStarRule).Name);
            mockReporter.DidNotReceive().ReportViolation(Arg.Any<IRuleViolation>());
        }

        [Test]
        public void RuleVisitorEnforcesOneRuleIgnoresAnother()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            mockReporter.ReportViolation(Arg.Any<IRuleViolation>());

            var mockConfigReader = Substitute.For<IConfigReader>();
            mockConfigReader.GetRuleSeverity("select-star").Returns(RuleViolationSeverity.Error);
            mockConfigReader.GetRuleSeverity("semicolon-termination").Returns(RuleViolationSeverity.Error);

            var ignoredRuleList = new List<IExtendedRuleException>
            {
                new RuleException(typeof(SelectStarRule), "select-star", 1, 10)
            };

            var pathString = "DoesntExist.sql";
            var ruleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter, RuleVisitorFriendlyNameTypeMap.Rules);
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO");
            var textReader = new StreamReader(testFileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);

            // act
            foreach (var sqlFragmentVisitor in activeRuleVisitors)
            {
                sqlFragment.Accept(sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(2, activeRuleVisitors.Count);
            mockReporter.Received().ReportViolation(Arg.Is<IRuleViolation>(x =>
                x.FileName == pathString
                && x.RuleName == "semicolon-termination"
                && x.Line == 1
                && x.Column == 18
                && x.Severity == RuleViolationSeverity.Error));
        }
    }
}
