using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigReaderTests
    {
        private string _testDirectory;

        private string TestDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_testDirectory))
                {
                    _testDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\UnitTests\Config");
                }
                return this._testDirectory;
            }
        }

        [Test]
        public void GetRuleSeverity()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var configReader = new ConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigNoRulesNoThrow()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-missing-rules");
            Assert.DoesNotThrow(() => { new ConfigReader(configfilePath); });
        }

        [Test]
        public void ConfigReadBadRuleName()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var configReader = new ConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("foo"));
        }

        [Test]
        public void ConfigReadBadRuleSeverity()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-severity");
            var configReader = new ConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
        }

        [Test]
        public void ConfigReadEmptyFile()
        {
            var configReader = new ConfigReader(string.Empty);
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void ConfigReadInvalidJson()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-json");
            var configReader = new ConfigReader(configfilePath);
            Assert.IsFalse(configReader.ConfigIsValid);
        }
    }
}