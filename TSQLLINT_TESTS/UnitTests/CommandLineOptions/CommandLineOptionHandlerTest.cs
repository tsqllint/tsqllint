using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_CONSOLE.ConfigHandler.Interfaces;
using TSQLLINT_CONSOLE.Reporters;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.CommandLineOptions
{
    public class CommandLineOptionHandlerTest
    {
        private readonly string DefaultConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc");

        [OneTimeSetUp]
        public void Setup()
        {
            var configFileGenerator = new ConfigFileGenerator(new ConsoleReporter());
            configFileGenerator.WriteConfigFile(DefaultConfigFile);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            File.Delete(DefaultConfigFile);
        }

        TestCommandLineOptionHandlerReporter reporter = new TestCommandLineOptionHandlerReporter();
        TestCommandLineOptionHandlerConfigFileGenerator configFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator();
        TestCommandLineOptionHandlerConfigFileFinder configFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(true);
        private readonly CommandLineOptionHandler Handler = new CommandLineOptionHandler();

        [Test]
        public void InitOptionsTest()
        {
            // arrange


            var args = new[]
            {
                "-i"
            };

            var options = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

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

            var options = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

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

            var options = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);
            var epectedMessage = "Config file not found. You may generate it with the \'--init\' option";

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

            var options = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var defaultConfigFile = Path.Combine(usersDirectory, @".tsqllintrc");
            var expectedMessage = string.Format("Config file found at: {0}", defaultConfigFile);

            // act
            Handler.HandleCommandLineOptions(options, configFileFinder, configFileGenerator, reporter);

            // assert
            Assert.AreEqual(1, reporter.Messages.Count);
            Assert.AreEqual(expectedMessage, reporter.Messages.First());
        }

        private class TestCommandLineOptionHandlerReporter : IBaseReporter
        {
            public readonly List<string> Messages = new List<string>();

            public void Report(string message)
            {
                Messages.Add(message);
            }
        }

        private class TestCommandLineOptionHandlerConfigFileFinder : IConfigFileFinder
        {
            private readonly bool ShouldFindFile;

            public TestCommandLineOptionHandlerConfigFileFinder(bool shouldFindFile)
            {
                ShouldFindFile = shouldFindFile;
            }

            public bool FindFile(string configFile)
            {
                return ShouldFindFile;
            }
        }

        private class TestCommandLineOptionHandlerConfigFileGenerator : IConfigFileGenerator
        {
            public readonly List<string> ConfigFilePaths = new List<string>();

            public void WriteConfigFile(string path)
            {
                ConfigFilePaths.Add(path);
            }
        }
    }
}