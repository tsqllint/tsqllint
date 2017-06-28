using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_TESTS.Unit_Tests.Config
{
    public class ConfigReaderTests
    {
        [Test]
        public void ReaderTests()
        {
            const string jsonConfig = @"
            {
                'rules': {
                    'select-star': 'error',
                    'statement-semicolon-termination': 'warning'
                }
            }";

            var ConfigReader = new LintConfigReader(jsonConfig);
            Assert.AreEqual(RuleViolationSeverity.Error, ConfigReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, ConfigReader.GetRuleSeverity("statement-semicolon-termination"));
        }
    }
}
