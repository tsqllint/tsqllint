using System;
using System.Linq;
using NUnit.Framework;

namespace TSQLLint.Tests.UnitTests.CommandLineOptions
{
    public class CommandLineParserTest
    {
        [Test]
        public void ParsesSingleFile()
        {
            // arrange
            const string path = @"c:\database\foo.sql";

            var args = new[]
            {
                "-c", "config.file",
                path
            };

            // act
            var commandLineOptions = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual("config.file", commandLineOptions.ConfigFile);
            Assert.AreEqual(path, commandLineOptions.LintPath.FirstOrDefault());
        }

        [Test]
        public void ParsesMultipleFiles()
        {
            // arrange
            const string fileOne = @"c:\database\foo.sql";
            const string fileTwo = @"c:\database\bar.sql";

            var args = new[]
            {
                fileOne,
                fileTwo
            };

            // act
            var commandLineOptions = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(fileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(fileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void ParsesMultipleFilesWithSpacesInDirectoryName()
        {
            // arrange
            const string fileOne = @"c:\database\foo.sql";
            const string fileTwo = @"c:\database directory\bar.sql";

            var args = new[]
            {
                fileOne,
                fileTwo
            };

            // act
            var commandLineOptions = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(fileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(fileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void GeneratesUsage()
        {
            // arrange
            var args = new string[0];

            // act
            var commandLineParser = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.IsTrue(commandLineParser.GetUsage().Contains("tsqllint [options]"));
        }

        [Test]
        public void ImplicitGetUsage()
        {
            // arrange
            var args = new string[0];

            // act
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var commandLineParser = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.IsTrue(commandLineParser.GetUsage().Contains("tsqllint [options]"));
        }

        [Test]
        public void ExplicitGetUsage()
        {
            // arrange
            var args = new[]
            {
                "-h"
            };

            // act
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var commandLineParser = new Console.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.IsTrue(commandLineParser.Help);
        }
    }
}
