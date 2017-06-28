using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Integration_Tests
{
    public class IntegrationTests
    {
        [Test]
        public void LintMultipleFiles()
        {
            var testDirectoryInfo = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            var result = testDirectoryInfo.Parent.Parent.FullName;

            var lintpathBase = Path.Combine(result + "\\IntegrationTests");
            var lintFileOne = Path.Combine(lintpathBase + "\\TestFiles\\integration-test-one.sql");
            var lintFileTwo = Path.Combine(lintpathBase + "\\TestFiles\\TestFileSubDirectory\\integration-test-two.sql");
            var lintTarget = Path.Combine(lintFileOne + ", " + lintFileTwo);

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintpathBase, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IReporter testReporter = new LintDirectoryTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, testReporter);

            fileProcessor.ProcessPath(lintTarget);
            testReporter.ReportResults(ruleVisitor.Violations, new TimeSpan(), 0);

            Assert.AreEqual(2, fileProcessor.GetFileCount());
            Assert.Throws<NotImplementedException>(() => { testReporter.Report(""); });
        }

        [Test]
        public void LintDirectory()
        {
            var lintBase = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\IntegrationTests");
            var lintTarget = Path.Combine(lintBase, "TestFiles");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintBase, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IReporter testReporter = new LintDirectoryTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, testReporter);

            fileProcessor.ProcessPath(lintTarget);
            testReporter.ReportResults(ruleVisitor.Violations, new TimeSpan(), 0);

            Assert.AreEqual(2, fileProcessor.GetFileCount());
            Assert.Throws<NotImplementedException>(() => { testReporter.Report(""); });
        }

        internal class LintDirectoryTestReporter : IReporter
        {
            public void ReportResults(List<RuleViolation> violations, TimeSpan timespan, int fileCount)
            {
                var selectStarViolations = violations.Where(x => x.RuleName == "select-star");
                Assert.AreEqual(2, selectStarViolations.Count(), "there should be two select star violation");
            }

            public void Report(string message)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void LintFile()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\IntegrationTests");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IReporter testReporter = new LintFileTestReporter();
            Assert.Throws<NotImplementedException>(() => { testReporter.Report(""); });
            var fileProcessor = new SqlFileProcessor(ruleVisitor, testReporter);

            var lintFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\IntegrationTests\\TestFiles\\integration-test-one.sql");
            fileProcessor.ProcessPath(lintFile);
            testReporter.ReportResults(ruleVisitor.Violations, new TimeSpan(), 0);
        }

        internal class LintFileTestReporter : IReporter
        {
            public void ReportResults(List<RuleViolation> ruleViolations, TimeSpan timespan, int fileCount)
            {
                var expectedRuleViolations = new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "conditional-begin-end", startLine: 2, startColumn: 1),
                    new RuleViolation(ruleName: "data-compression", startLine: 6, startColumn: 1),
                    new RuleViolation(ruleName: "data-type-length", startLine: 13, startColumn: 16),
                    new RuleViolation(ruleName: "disallow-cursors", startLine: 17, startColumn: 1),
                    new RuleViolation(ruleName: "information-schema", startLine: 20, startColumn: 27),
                    new RuleViolation(ruleName: "keyword-capitalization", startLine: 23, startColumn: 1),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 27, startColumn: 10),
                    new RuleViolation(ruleName: "object-property", startLine: 38, startColumn: 7),
                    new RuleViolation(ruleName: "print-statement", startLine: 42, startColumn: 1),
                    new RuleViolation(ruleName: "schema-qualify", startLine: 45, startColumn: 17),
                    new RuleViolation(ruleName: "select-star", startLine: 48, startColumn: 8),
                    new RuleViolation(ruleName: "semicolon-termination", startLine: 51, startColumn: 31),
                    new RuleViolation(ruleName: "set-ansi", startLine: 1, startColumn: 1),
                    new RuleViolation(ruleName: "set-nocount", startLine: 1, startColumn: 1),
                    new RuleViolation(ruleName: "set-quoted-identifier", startLine: 1, startColumn: 1),
                    new RuleViolation(ruleName: "set-transaction-isolation-level", startLine: 1, startColumn: 1),
                    new RuleViolation(ruleName: "upper-lower", startLine: 59, startColumn: 8),
                };

                ruleViolations = ruleViolations.OrderBy(o => o.RuleName).ToList();
                expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.RuleName).ToList();

                var ruleCompare = new RuleViolationCompare();

                // assert
                Assert.AreEqual(expectedRuleViolations.Count, ruleViolations.Count);
                CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, ruleCompare);
            }

            public void Report(string message)
            {
                throw new NotImplementedException();
            }
        }
    }
}