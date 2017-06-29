using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Integration_Tests
{
    public class IntegrationHappyPathCases
    {

        [Test]
        public void RuleValidationTest()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\HappyPath");

            var configFileString = File.ReadAllText(Path.Combine(lintTarget, ".tsqllintrc"));

            ILintConfigReader configReader = new LintConfigReader(configFileString);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            IResultReporter testReporter = new TestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            fileProcessor.ProcessPath(lintTarget);
            testReporter.ReportResults(ruleVisitor.Violations);
        }
    }

    internal class TestReporter : IResultReporter
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
}