using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.IntegrationTests.RuleExceptions
{
    public class RuleVisitorBuilderIgnoreRulesTests
    {
        [Test]
        public void RuleVisitorEnforcesRule()
        {
            // arrange
            var mockReporter = Substitute.For<IReporter>();
            mockReporter.ReportViolation(Arg.Any<IRuleViolation>());
            
            var mockConfigReader = Substitute.For<IConfigReader>();
            mockConfigReader.GetRuleSeverity("select-star").Returns(RuleViolationSeverity.Error);

            var ignoredRuleList = new List<IRuleException>();

            var pathString = "DoesntExist.sql";
            var RuleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter);
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO;");
            var fragmentVisitor = new SqlRuleVisitor(mockConfigReader, mockReporter);

            // act
            foreach (var sqlFragmentVisitor in ActiveRuleVisitors)
            {
                fragmentVisitor.VisitRule(testFileStream, sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(1, ActiveRuleVisitors.Count);
            Assert.IsTrue(ActiveRuleVisitors[0].GetType().Name == typeof(SelectStarRule).Name);
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

            var ignoredRuleList = new List<IRuleException>
            {
                new RuleException(typeof(SelectStarRule), 1, 10)
            };

            var pathString = "DoesntExist.sql";
            var RuleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter);
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO;");
            var fragmentVisitor = new SqlRuleVisitor(mockConfigReader, mockReporter);

            // act
            foreach (var sqlFragmentVisitor in ActiveRuleVisitors)
            {
                fragmentVisitor.VisitRule(testFileStream, sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(1, ActiveRuleVisitors.Count);
            Assert.IsTrue(ActiveRuleVisitors[0].GetType().Name == typeof(SelectStarRule).Name);
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

            var ignoredRuleList = new List<IRuleException>
            {
                new RuleException(typeof(SelectStarRule), 1, 10)
            };

            var pathString = "DoesntExist.sql";
            var RuleVisitorBuilder = new RuleVisitorBuilder(mockConfigReader, mockReporter);
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors(pathString, ignoredRuleList);
            var testFileStream = ParsingUtility.GenerateStreamFromString("SELECT * FROM FOO");
            var fragmentVisitor = new SqlRuleVisitor(mockConfigReader, mockReporter);

            // act
            foreach (var sqlFragmentVisitor in ActiveRuleVisitors)
            {
                fragmentVisitor.VisitRule(testFileStream, sqlFragmentVisitor);
                testFileStream.Seek(0, SeekOrigin.Begin);
            }

            // assert
            Assert.AreEqual(2, ActiveRuleVisitors.Count);
            mockReporter.Received().ReportViolation(Arg.Is<IRuleViolation>(x =>
                x.FileName == pathString
                && x.RuleName == "semicolon-termination"
                && x.Line == 1
                && x.Column == 18
                && x.Severity == RuleViolationSeverity.Error));
        }
    }
}
