using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;
using TSQLLINT_LIB_TESTS.Integration_Tests;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.Config
{
    public class ConfigReaderTests
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [Test]
        public void ReaderTests()
        {
            var configString = TestHelper.GetTestFile("Config\\tsqllintrc");
            var ConfigReader = new LintConfigReader(configString);
            Assert.AreEqual(RuleViolationSeverity.Error, ConfigReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, ConfigReader.GetRuleSeverity("statement-semicolon-termination"));
        }
    }
}
