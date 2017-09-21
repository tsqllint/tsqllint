using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_CONSOLE.ConfigHandler;

namespace TSQLLINT_LIB_TESTS.UnitTests.CommandLineOptions
{
    public class CommandLineParserTest
    {
        private string _configFilePath;
        private string ConfigFilePath
        {
            get { return (string.IsNullOrWhiteSpace(_configFilePath) == false) ? _configFilePath : InitializeConfigFilePath(); }
        }

        private string InitializeConfigFilePath()
        {
            var testDirectoryInfo = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            var result = testDirectoryInfo.Parent.Parent.FullName;
            _configFilePath = Path.Combine(result + @"\IntegrationTests\.tsqllintrc");

            return _configFilePath;
        }

        [Test]
        public void NoProblems()
        {
            // arrange

            var path = @"c:\database\foo.sql";

            var args = new[]
            {
                "-c", ConfigFilePath,
                path
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(ConfigFilePath, commandLineOptions.ConfigFile);
            Assert.AreEqual(path, commandLineOptions.LintPath.FirstOrDefault());
        }

        [Test]
        public void MultipleFiles()
        {
            // arrange
            var fileOne = @"c:\database\foo.sql";
            var fileTwo = @"c:\database\ActivatePlugins_ReportViolations_ShouldCallReporter.sql";

            var args = new[]
            {
                fileOne,
                fileTwo
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(fileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(fileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void MultipleFilesWithSpaces()
        {
            // arrange
            var fileOne = @"c:\database\foo.sql";
            var fileTwo = @"c:\database directory\ActivatePlugins_ReportViolations_ShouldCallReporter.sql";

            var args = new[]
            {
                fileOne,
                fileTwo
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(fileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(fileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void DefaultConfigFile()
        {
            // arrange
            var args = new string[0];

            // act
            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var commandLineParser = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            //assert
            Assert.AreEqual(Path.Combine(usersDirectory, @".tsqllintrc"), commandLineParser.ConfigFile);
        }

        [Test]
        public void GetUsage()
        {
            // arrange
            var args = new string[0];

            // act
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var commandLineParser = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            //assert
            Assert.IsTrue(commandLineParser.GetUsage().Contains("tsqllint [options]"));
        }
    }
}