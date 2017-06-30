using System;
using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.Config
{
    public class ConfigReaderTests
    {
        [Test]
        public void GetRuleSeverity()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Unit Tests\\Config\\.tsqllintrc" );
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Error, ConfigReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, ConfigReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReadEmptyFile()
        {
            Assert.Throws<Exception>(() => { new LintConfigReader(""); });
        }

        [Test]
        public void ConfigReadBadRule()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Unit Tests\\Config\\.tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, ConfigReader.GetRuleSeverity("foo"));
        }
    }
}
