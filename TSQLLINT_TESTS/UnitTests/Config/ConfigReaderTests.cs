using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigReaderTests
    {
        private string _TestDirectory;
        private string TestDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TestDirectory))
                {
                    _TestDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\UnitTests\Config");
                }
                return _TestDirectory;
            }

        }

        [Test]
        public void GetRuleSeverity()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Error, ConfigReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, ConfigReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigNoRulesNoThrow()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-missing-rules");
            Assert.DoesNotThrow(() => { new LintConfigReader(configfilePath); });
        }

        [Test]
        public void ConfigReadBadRuleName()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, ConfigReader.GetRuleSeverity("foo"));
        }

        [Test]
        public void ConfigReadBadRuleSeverity()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-severity");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, ConfigReader.GetRuleSeverity("select-star"));
        }

        [Test]
        public void ConfigReadEmptyFile()
        {
            var ConfigReader = new LintConfigReader("");
            Assert.IsFalse(ConfigReader.ConfigIsValid);
        }

        [Test]
        public void ConfigReadInvalidJson()
        {
            var configfilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-json");
            Assert.Throws<JsonReaderException>(() => { new LintConfigReader(configfilePath); });
        }
    }
}