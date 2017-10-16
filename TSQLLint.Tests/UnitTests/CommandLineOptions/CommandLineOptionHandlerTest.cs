using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Console.ConfigHandler.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Tests.UnitTests.CommandLineOptions
{
    public class CommandLineOptionHandlerTest
    {
        [Test]
        public void Prints_Version_Information_When_Requested()
        {
            // arrange
            var info = SetupHandler(new[] { "-v" });

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            var tsqllintVersion = $"v{version}";

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(tsqllintVersion, info.Reporter.Messages.First());
        }

        [Test]
        public void Prints_Config_Information_When_Requested_For_ConfigFile()
        {
            // arrange
            const string expectedMessage = "Config file found at: .tsqllintrc";
            var info = SetupHandler(new[] { "-p" });

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(expectedMessage, info.Reporter.Messages.First());
        }

        [Test]
        public void Prints_Config_Information_When_Requested_For_InMemory()
        {
            // arrange
            const string expectedMessage = "Using default config instead of a file";
            var info = SetupHandler(new[] { "-p" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(expectedMessage, info.Reporter.Messages.First());
        }

        [Test]
        public void Returns_In_Memory_Config_If_Missing_Config_File_When_None_Passed_And_No_Options()
        {
            // arrange
            var info = SetupHandler(new List<string>().ToArray(), false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.IsNull(info.Options.ConfigFile);
        }

        [Test]
        public void Reports_Error_If_Provided_Invalid_Config_File_And_No_Options()
        {
            // arrange
            const string expectedMessage = "Config file not found at: doesnotexist.config use the '--init' option to create if one does not exist or the '--force' option to overwrite";
            var info = SetupHandler(new[] { "-c", "doesnotexist.config", "file1.sql" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsTrue(performLinting);
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(expectedMessage, info.Reporter.Messages.First());
        }

        [Test]
        public void Creates_Default_Config_File_If_Does_Not_Exist_And_Init_Option_Is_Used_And_No_Config_Option_Is_Used()
        {
            // arrange
            var info = SetupHandler(new[] { "-i" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Specified_Config_File_If_Does_Not_Exist_And_Init_Option_Is_Used_And_Config_Option_Is_Used()
        {
            // arrange
            var info = SetupHandler(new[] { "-i", "-c", "custom.config" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        [Test]
        public void Does_Not_Create_Config_File_If_Exists_When_Using_Init_Option()
        {
            // arrange
            var info = SetupHandler(new[] { "-i" });

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsFalse(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Default_Config_File_When_Using_Force_Option_And_Config_Does_Not_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Specified_Config_File_When_Using_Force_Option_And_Config_Option_And_Config_Does_Not_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f", "-c", "custom.config" }, false);

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        [Test]
        public void Overwrites_Default_Config_File_When_Using_Force_Option_And_Config_Does_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f" });

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Overwrites_Specified_Config_File_When_Using_Force_Option_And_Config_Option_And_Config_Does_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f", "-c", "custom.config" });

            // act
            var performLinting = info.Handler.HandleCommandLineOptions();

            // assert
            Assert.IsFalse(performLinting);
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        private static TestObjects SetupHandler(string[] args, bool shouldFindFile = true, string defaultConfigFile = ".tsqllintrc")
        {
            var info = new TestObjects
            {
                Options = new Console.ConfigHandler.CommandLineOptions(args),
                Reporter = new TestCommandLineOptionHandlerReporter(),
                ConfigFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator(),
                ConfigFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(shouldFindFile, defaultConfigFile)
            };
            info.Handler = new CommandLineOptionHandler(info.Options, info.ConfigFileFinder, info.ConfigFileGenerator, info.Reporter);
            return info;
        }

        private class TestObjects
        {
            public Console.ConfigHandler.CommandLineOptions Options { get; set; }

            public TestCommandLineOptionHandlerReporter Reporter { get; set; }

            public TestCommandLineOptionHandlerConfigFileGenerator ConfigFileGenerator { get; set; }

            public TestCommandLineOptionHandlerConfigFileFinder ConfigFileFinder { get; set; }

            public CommandLineOptionHandler Handler { get; set; }
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
            private readonly bool _shouldFindFile;

            public TestCommandLineOptionHandlerConfigFileFinder(bool shouldFindFile, string defaultConfigFileName)
            {
                _shouldFindFile = shouldFindFile;
                DefaultConfigFileName = defaultConfigFileName;
            }

            public string DefaultConfigFileName { get; }

            public bool FindFile(string configFile)
            {
                return _shouldFindFile;
            }
        }

        private class TestCommandLineOptionHandlerConfigFileGenerator : IConfigFileGenerator
        {
            public readonly List<string> ConfigFilePathsWritten = new List<string>();

            private int DefaultConfigRuleCalledCount
            {
                get;
                set;
            }

            public string GetDefaultConfigRules()
            {
                DefaultConfigRuleCalledCount++;
                return "Some Rules";
            }

            public void WriteConfigFile(string path)
            {
                ConfigFilePathsWritten.Add(path);
            }
        }
    }
}
