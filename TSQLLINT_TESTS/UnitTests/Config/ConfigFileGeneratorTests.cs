using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigFileGeneratorTests
    {
        private string _configFileName;

        private string ConfigFileName
        {
            get { return (string.IsNullOrWhiteSpace(_configFileName)) ? SetConfigFile() : _configFileName; }
        }

        public string SetConfigFile()
        {
            _configFileName = Path.Combine(TestContext.CurrentContext.WorkDirectory, ".tsqllintrc");
            return _configFileName;
        }

        [Test]
        public void WriteConfigFile()
        {
            // arrange
            var reporter = new TestReporter(); 
            var configFileGenerator = new ConfigFileGenerator(reporter);

            // act
            configFileGenerator.WriteConfigFile(ConfigFileName);

            // assert
            Assert.IsTrue(File.Exists(ConfigFileName));
            Assert.AreEqual(1, reporter.MessageCount);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(ConfigFileName);
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