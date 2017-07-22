using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigReaderTests
    {
        [Test]
        public void GetRuleSeverity()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\Config\\.tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Error, ConfigReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, ConfigReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigNoRulesNoThrow()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\Config\\.tsqllintrc-missing-rules");
            Assert.DoesNotThrow(() => { new LintConfigReader(configfilePath); });
        }

        [Test]
        public void ConfigReadBadRuleName()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\Config\\.tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, ConfigReader.GetRuleSeverity("foo"));
        }

        [Test]
        public void ConfigReadBadRuleSeverity()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\Config\\.tsqllintrc-bad-severity");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, ConfigReader.GetRuleSeverity("select-star"));
        }

        [Test]
        public void ConfigReadEmptyFile()
        {
            Assert.Throws<Exception>(() => { new LintConfigReader(""); });
        }

        [Test]
        public void ConfigReadInvalidJson()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\Config\\.tsqllintrc-bad-json");
            Assert.Throws<JsonReaderException>(() => { new LintConfigReader(configfilePath); });
        }
    }
}