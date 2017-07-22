using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TSQLLINT_CONSOLE.CommandLineOptions;
using TSQLLINT_CONSOLE.CommandLineParser;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.CommandLineParser
{
    class CommandLineOptionHandlerTest
    {
        CommandLineOptionHandler Handler = new CommandLineOptionHandler();

        [Test]
        public void InitOptionsTest()
        {
            // arrange
            var reporter = new TestCommandLineOptionHandlerReporter();
            var configFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator();
            var configFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(true);

            var args = new[]
            {
                "-i"
            };

            var options = new ConsoleCommandLineOptionParser(args, reporter);

            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var configFilePath = Path.Combine(usersDirectory, @".tsqllintrc");

            // act
            Handler.HandleCommandLineOptions(options, configFileFinder, configFileGenerator, reporter);

            // assert
            Assert.AreEqual(1, configFileGenerator.ConfigFilePaths.Count);
            Assert.AreEqual(configFilePath, configFileGenerator.ConfigFilePaths.First());
        }

        [Test]
        public void VersionOptionsTest()
        {
            // arrange
            var reporter = new TestCommandLineOptionHandlerReporter();
            var configFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator();
            var configFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(true);


            var args = new[]
            {
                "-v"
            };

            var options = new ConsoleCommandLineOptionParser(args, reporter);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            var tsqllintVersion = string.Format("v{0}", version);

            // act
            Handler.HandleCommandLineOptions(options, configFileFinder, configFileGenerator, reporter);

            // assert
            Assert.AreEqual(1, reporter.Messages.Count);
            Assert.AreEqual(tsqllintVersion, reporter.Messages.First());
        }

        [Test]
        public void PrintConfigOptionsFileNotExistTest()
        {
            // arrange
            var reporter = new TestCommandLineOptionHandlerReporter();
            var configFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator();
            var configFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(false);


            var args = new[]
            {
                "-p"
            };

            var options = new ConsoleCommandLineOptionParser(args, reporter);
            var epectedMessage = "Default config file not found. You may generate it with the \'--init\' option";

            // act
            Handler.HandleCommandLineOptions(options, configFileFinder, configFileGenerator, reporter);

            // assert
            Assert.AreEqual(1, reporter.Messages.Count);
            Assert.AreEqual(epectedMessage, reporter.Messages.First());
        }

        [Test]
        public void PrintConfigOptionsFileExistTest()
        {
            // arrange
            var reporter = new TestCommandLineOptionHandlerReporter();
            var configFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator();
            var configFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(true);


            var args = new[]
            {
                "-p"
            };

            var options = new ConsoleCommandLineOptionParser(args, reporter);

            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var defaultConfigFile = Path.Combine(usersDirectory, @".tsqllintrc");
            var expectedMessage = string.Format("Default config file found at: {0}", defaultConfigFile);

            // act
            Handler.HandleCommandLineOptions(options, configFileFinder, configFileGenerator, reporter);

            // assert
            Assert.AreEqual(1, reporter.Messages.Count);
            Assert.AreEqual(expectedMessage, reporter.Messages.First());
        }

        private class TestCommandLineOptionHandlerReporter : IBaseReporter
        {
            public List<string> Messages = new List<string>();

            public void Report(string message)
            {
                Messages.Add(message);
            }
        }

        private class TestCommandLineOptionHandlerConfigFileFinder : IConfigFileFinder
        {
            private bool ShouldFindFile;

            public TestCommandLineOptionHandlerConfigFileFinder(bool shouldFindFile)
            {
                ShouldFindFile = shouldFindFile;
            }

            public bool FindDefaultConfigFile(string configFile)
            {
                return ShouldFindFile;
            }
        }

        private class TestCommandLineOptionHandlerConfigFileGenerator : IConfigFileGenerator
        {
            public List<string> ConfigFilePaths = new List<string>();

            public void WriteConfigFile(string path)
            {
                ConfigFilePaths.Add(path);
            }
        }
    }
}