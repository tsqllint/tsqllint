using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Integration_Tests.HappyPath
{
    public class IntegrationHappyPathCases
    {
        [Test]
        public void HappyPathLintDirectory()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IResultReporter testReporter = new HappyPathLintDirectoryTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor);

            fileProcessor.ProcessPath(lintTarget);
            testReporter.ReportResults(ruleVisitor.Violations);

            Assert.AreEqual(2, fileProcessor.GetFileCount());
        }

        internal class HappyPathLintDirectoryTestReporter : IResultReporter
        {
            public void ReportResults(List<RuleViolation> violations)
            {
                var selectStarViolations = violations.Where(x => x.RuleName == "select-star");
                Assert.AreEqual(2, selectStarViolations.Count(), "there should be two select star violation");
            }
        }

        [Test]
        public void HappyPathLintFile()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IResultReporter testReporter = new HappyPathLintFileTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor);

            var lintFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath\\Test Files\\happy-path-one.sql");
            fileProcessor.ProcessPath(lintFile);
            testReporter.ReportResults(ruleVisitor.Violations);
        }

        internal class HappyPathLintFileTestReporter : IResultReporter
        {
            public void ReportResults(List<RuleViolation> violations)
            {
                var conditionalBeginEnd = violations.Where(x => x.RuleName == "conditional-begin-end");
                Assert.AreEqual(1, conditionalBeginEnd.Count(), "there should be one conditional-begin-end violation");

                var dataCompressionViolations = violations.Where(x => x.RuleName == "data-compression");
                Assert.AreEqual(1, dataCompressionViolations.Count(), "there should be one data-compression violation");

                var dataTypeLength = violations.Where(x => x.RuleName == "data-type-length");
                Assert.AreEqual(1, dataTypeLength.Count(), "there should be one data-type-length violation");

                var disallowCursors = violations.Where(x => x.RuleName == "disallow-cursors");
                Assert.AreEqual(1, disallowCursors.Count(), "there should be one disallow-cursors violation");

                var informationSchema = violations.Where(x => x.RuleName == "information-schema");
                Assert.AreEqual(1, informationSchema.Count(), "there should be one information-schema violation");

                var objectProperty = violations.Where(x => x.RuleName == "object-property");
                Assert.AreEqual(1, objectProperty.Count(), "there should be one object-property violation");

                var printStatement = violations.Where(x => x.RuleName == "print-statement");
                Assert.AreEqual(1, printStatement.Count(), "there should be one print-statement violation");

                var schemaQualify = violations.Where(x => x.RuleName == "schema-qualify");
                Assert.AreEqual(1, schemaQualify.Count(), "there should be one schema-qualify violation");

                var selectStar = violations.Where(x => x.RuleName == "select-star");
                Assert.AreEqual(1, selectStar.Count(), "there should be one select-star violation");

                var statementSemicolonTermination = violations.Where(x => x.RuleName == "semicolon-termination");
                Assert.AreEqual(1, statementSemicolonTermination.Count(), "there should be one statement-semicolon-termination violation");

                var setAnsi = violations.Where(x => x.RuleName == "set-ansi");
                Assert.AreEqual(1, setAnsi.Count(), "there should be one set-ansi violation");

                var setNocount = violations.Where(x => x.RuleName == "set-nocount");
                Assert.AreEqual(1, setNocount.Count(), "there should be one set-nocount violation");

                var setQuoted = violations.Where(x => x.RuleName == "set-quoted-identifier");
                Assert.AreEqual(1, setQuoted.Count(), "there should be one set-quoted-identifier violation");

                var setTransactionIsolationLevel = violations.Where(x => x.RuleName == "set-transaction-isolation-level");
                Assert.AreEqual(1, setTransactionIsolationLevel.Count(), "there should be one set-transaction-isolation-level violation");

                var upperLower = violations.Where(x => x.RuleName == "upper-lower");
                Assert.AreEqual(1, upperLower.Count(), "there should be one upper-lower violation");
            }
        }
    }
}