using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Lib.Config;

namespace TSQLLint.Tests.UnitTests.Config
{
    public class ConfigFileGeneratorTests
    {
        private string _configFileName;

        private string ConfigFileName => string.IsNullOrWhiteSpace(_configFileName) ? SetConfigFile() : _configFileName;

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
            var mockReporter = Substitute.For<IReporter>();

            var reportedMessages = new List<string>();
            mockReporter.When(reporter => reporter.Report(Arg.Any<string>())).Do(x => reportedMessages.Add(x.Arg<string>()));
            
            var configFileGenerator = new ConfigFileGenerator(mockReporter);
            var configFileFinder = new ConfigFileFinder();

            // act
            configFileGenerator.WriteConfigFile(ConfigFileName);

            // assert
            Assert.IsTrue(configFileFinder.FindFile(ConfigFileName));
            Assert.AreEqual(1, reportedMessages.Count);
        }

        [Test]
        public void GetDefaultConfigRules_Returns_Default_Rules()
        {
            // arrange
            var testReporter = Substitute.For<IReporter>();

            var reportedMessages = new List<string>();
            testReporter.When(reporter => reporter.Report(Arg.Any<string>())).Do(x => reportedMessages.Add(x.Arg<string>()));
            
            var configFileGenerator = new ConfigFileGenerator(testReporter);

            // act
            var defaultRules = configFileGenerator.GetDefaultConfigRules();

            // assert
            StringAssert.Contains("rules", defaultRules);
            Assert.AreEqual(1, reportedMessages.Count);
        }
    }
}
