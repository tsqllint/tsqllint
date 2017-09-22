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
                return _testDirectory;
            }
        }

        [Test]
        public void LoadConfigFromFile_Does_Not_Throw_Error_When_ConfigFile_Parameter_Is_Missing()
        {
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(string.Empty);
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromFile_Does_Not_Throw_Error_When_ConfigFile_Does_Not_Exist()
        {
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(".tsqllintrc-doesnot_exist");
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromFile_Does_Not_Throw_Error_When_ConfigFile_Is_Invalid_Json()
        {
            var configFilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-json");
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(configFilePath);
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromFile_Does_Not_Throw_Error_When_ConfigFile_Has_No_Rules()
        {
            var configFilePath = Path.Combine(TestDirectory, ".tsqllintrc-missing-rules");
            var configReader = new ConfigReader();
            Assert.DoesNotThrow(() => { configReader.LoadConfigFromFile(configFilePath); });
            Assert.IsTrue(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromRules_Does_Not_Throw_Error_When_ConfigFile_Parameter_Is_Missing()
        {
            var configReader = new ConfigReader();
            configReader.LoadConfigFromRules(string.Empty);
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromRules_Does_Not_Throw_Error_When_ConfigFile_Is_Invalid_Json()
        {
            var configReader = new ConfigReader();
            configReader.LoadConfigFromRules("{");
            Assert.IsFalse(configReader.ConfigIsValid);
        }

        [Test]
        public void LoadConfigFromRules_Does_Not_Throw_Error_When_ConfigFile_Has_No_Rules()
        {
            var configReader = new ConfigReader();
            Assert.DoesNotThrow(() => { configReader.LoadConfigFromRules("{}"); });
            Assert.IsTrue(configReader.ConfigIsValid);
        }

        [Test]
        public void GetRuleSeverity_Returns_Correct_Severity_From_ConfigFile()
        {
            var configFilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(configFilePath);
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void GetRuleSeverity_Returns_Correct_No_Severity_When_Passed_Unknown_Rule()
        {
            var configFilePath = Path.Combine(TestDirectory, ".tsqllintrc");
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(configFilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("foo"));
        }

        [Test]
        public void GetRuleSeverity_Returns_Correct_No_Severity_When_Severity_Value_Is_Invalid()
        {
            var configFilePath = Path.Combine(TestDirectory, ".tsqllintrc-bad-severity");
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(configFilePath);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
        }
    }
}