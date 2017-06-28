using System;
using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_TESTS.Integration_Tests
{
    public class IntegrationExceptionCases
    {
        private string workingDirectory;
        private string configFilePath;
        private TestHelper TestHelper;

        [OneTimeSetUp]
        public void setup()
        {
            workingDirectory = TestContext.CurrentContext.TestDirectory;
            TestHelper = new TestHelper(workingDirectory);

            TestHelper.createFileForTesting(testFileName: @"IntegrationExceptionCase.sql", content: @"
                BEGIN
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
        public void RuleValidationTestShouldThrow()
        {
            var configFileString = File.ReadAllText(configFilePath);

            ILintConfigReader configReader = new LintConfigReader(configFileString);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            var ex = Assert.Throws<Exception>(() => fileProcessor.ProcessPath(workingDirectory));
        }
    }
}