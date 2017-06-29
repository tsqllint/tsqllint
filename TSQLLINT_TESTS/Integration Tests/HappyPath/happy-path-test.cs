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

            var configFileString = File.ReadAllText(Path.Combine(lintTarget, ".tsqllintrc"));

            ILintConfigReader configReader = new LintConfigReader(configFileString);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            IResultReporter testReporter = new HappyPathLintDirectoryTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            fileProcessor.ProcessPath(lintTarget);
            testReporter.ReportResults(ruleVisitor.Violations);
        }

        internal class HappyPathLintDirectoryTestReporter : IResultReporter
        {
            public void ReportResults(List<RuleViolation> violations)
            {
                var selectStarViolations = violations.Where(x => x.RuleName == "select-star");
                Assert.AreEqual(1, selectStarViolations.Count(), "there should be one select star violation");

                var semicolonViolations = violations.Where(x => x.RuleName == "statement-semicolon-termination");
                Assert.AreEqual(2, semicolonViolations.Count(), "there should be two semicolon violations");

                var transactionLevel = violations.Where(x => x.RuleName == "set-transaction-isolation-level");
                Assert.AreEqual(2, transactionLevel.Count(), "there should be one violation of the transaction isolation level");
            }
        }

        [Test]
        public void HappyPathLintFile()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath");

            var configFileString = File.ReadAllText(Path.Combine(lintTarget, ".tsqllintrc"));

            ILintConfigReader configReader = new LintConfigReader(configFileString);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            IResultReporter testReporter = new HappyPathLintFileTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            var lintFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath\\Test Files\\happy-path-one.sql");
            fileProcessor.ProcessPath(lintFile);
            testReporter.ReportResults(ruleVisitor.Violations);
        }

        internal class HappyPathLintFileTestReporter : IResultReporter
        {
            public void ReportResults(List<RuleViolation> violations)
            {
                Assert.AreEqual(2, violations.Count);

                var semicolonViolations = violations.Where(x => x.RuleName == "statement-semicolon-termination");
                Assert.AreEqual(1, semicolonViolations.Count(), "there should be two semicolon violations");

                var transactionLevel = violations.Where(x => x.RuleName == "set-transaction-isolation-level");
                Assert.AreEqual(1, transactionLevel.Count(), "there should be one violation of the transaction isolation level");
            
            }
        }
    }


}