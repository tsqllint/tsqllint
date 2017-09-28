using System.IO;
using NUnit.Framework;
using TSQLLINT_COMMON;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigFileGeneratorTests
    {
        private string _configFileName;

        private string ConfigFileName
        {
            get { return string.IsNullOrWhiteSpace(_configFileName) ? SetConfigFile() : _configFileName; }
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(ConfigFileName);
        }

        private string SetConfigFile()
        {
            _configFileName = Path.Combine(TestContext.CurrentContext.WorkDirectory, ".tsqllintrc");
            return _configFileName;
        }

        [Test]
        public void WriteConfigFile_Writes_To_File()
        {
            // arrange
            var reporter = new TestReporter(); 
            var configFileGenerator = new ConfigFileGenerator(reporter);
            var configFileFinder = new ConfigFileFinder();

            // act
            configFileGenerator.WriteConfigFile(ConfigFileName);

            // assert
            Assert.IsTrue(configFileFinder.FindFile(ConfigFileName));
            Assert.AreEqual(1, reporter.MessageCount);
        }

        [Test]
        public void GetDefaultConfigRules_Returns_Default_Rules()
        {
            // arrange
            var reporter = new TestReporter();
            var configFileGenerator = new ConfigFileGenerator(reporter);

            // act
            var defaultRules = configFileGenerator.GetDefaultConfigRules();

            // assert
            StringAssert.Contains("rules", defaultRules);
            Assert.AreEqual(1, reporter.MessageCount);
        }

        private class TestReporter : IBaseReporter
        {
            public int MessageCount;
            public void Report(string message)
            {
                MessageCount++;
            }
        }
    }
}