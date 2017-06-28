using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_TESTS.Integration_Tests
{
    public class IntegrationHappyPathCases
    {
        private string workingDirectory;
        private string configFilePath;
        private TestHelper TestHelper;

        [OneTimeSetUp]
        public void setup()
        {
            workingDirectory = TestContext.CurrentContext.TestDirectory;
            TestHelper = new TestHelper(workingDirectory);

            TestHelper.createFileForTesting(testFileName: @"IntegrationTestFile1.sql", content: @"
                    update foo set bar = 1
                    select foo from bar
            ");

            TestHelper.createFileForTesting(testFileName: @"IntegrationTestFile2.sql", content: @"
                    select * from foo;
            ");

            TestHelper.createFileForTesting(testFileName: @".tsqllintrc", content: @"
                    {
                      'rules': {
                        'select-star': 'error',
                        'statement-semicolon-termination': 'error',
                        'set-transaction-isolation-level': 'error'
                      }
                    }
            ");

            configFilePath = Path.GetFullPath(Path.Combine(workingDirectory, @".tsqllintrc"));
        }

        [OneTimeTearDown]
        public void teardown()
        {
            TestHelper.cleanup();
        }

        [Test]
        public void RuleValidationTest()
        {
            var configFileString = File.ReadAllText(configFilePath);

            ILintConfigReader configReader = new LintConfigReader(configFileString);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            IResultReporter testReporter = new TestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            fileProcessor.ProcessPath(workingDirectory);
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