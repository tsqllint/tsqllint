using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_CONSOLE.ConfigHandler.Interfaces;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.CommandLineOptions
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
            var tsqllintVersion = string.Format("v{0}", version);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(tsqllintVersion, info.Reporter.Messages.First());
        }

        [Test]
        public void Prints_Config_Information_When_Requested()
        {
            // arrange
            const string ExpectedMessage = "Config file found at: .tsqllintrc";
            var info = SetupHandler(new[] { "-p" });

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(ExpectedMessage, info.Reporter.Messages.First());
        }

        [Test]
        public void Reports_Error_If_Missing_Config_File_When_None_Passed_And_No_Options()
        {
            // arrange
            const string ExpectedMessage = "Existing config file not found at: .tsqllintrc use the '--init' option to create if one does not exist or the '--force' option to overwrite";
            var info = SetupHandler(new[] { "file1.sql" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(ExpectedMessage, info.Reporter.Messages.First());            
        }

        [Test]
        public void Reports_Error_If_Provided_Invalid_Config_File_And_No_Options()
        {
            // arrange
            const string ExpectedMessage = "Existing config file not found at: doesnotexist.config use the '--init' option to create if one does not exist or the '--force' option to overwrite";
            var info = SetupHandler(new[] { "-c", "doesnotexist.config", "file1.sql" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(1, info.Reporter.Messages.Count);
            Assert.AreEqual(ExpectedMessage, info.Reporter.Messages.First());
        }

        [Test]
        public void Creates_Default_Config_File_If_Does_Not_Exist_And_Init_Option_Is_Used_And_No_Config_Option_Is_Used()
        {
            // arrange
            var info = SetupHandler(new[] { "-i" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Specified_Config_File_If_Does_Not_Exist_And_Init_Option_Is_Used_And_Config_Option_Is_Used()
        {
            // arrange
            var info = SetupHandler(new[] { "-i", "-c", "custom.config" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        [Test]
        public void Does_Not_Create_Config_File_If_Exists_When_Using_Init_Option()
        {
            // arrange
            var info = SetupHandler(new[] { "-i" });

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsFalse(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Default_Config_File_When_Using_Force_Option_And_Config_Does_Not_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Creates_Specified_Config_File_When_Using_Force_Option_And_Config_Option_And_Config_Does_Not_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f", "-c", "custom.config" }, shouldFindFile: false);

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        [Test]
        public void Overwrites_Default_Config_File_When_Using_Force_Option_And_Config_Does_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f" });

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual(".tsqllintrc", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains(".tsqllintrc"));
        }

        [Test]
        public void Overwrites_Specified_Config_File_When_Using_Force_Option_And_Config_Option_And_Config_Does_Exist()
        {
            // arrange
            var info = SetupHandler(new[] { "-f", "-c", "custom.config" });

            // act
            info.Handler.HandleCommandLineOptions();

            // assert
            Assert.AreEqual("custom.config", info.Options.ConfigFile);
            Assert.IsTrue(info.ConfigFileGenerator.ConfigFilePathsWritten.Contains("custom.config"));
        }

        private static TestObjects SetupHandler(string[] args, bool shouldFindFile = true, string defaultConfigFile = ".tsqllintrc")
        {
            var info = new TestObjects
            {
                Options = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args),
                Reporter = new TestCommandLineOptionHandlerReporter(),
                ConfigFileGenerator = new TestCommandLineOptionHandlerConfigFileGenerator(),
                ConfigFileFinder = new TestCommandLineOptionHandlerConfigFileFinder(shouldFindFile, defaultConfigFile)
            };
            info.Handler = new CommandLineOptionHandler(info.Options, info.ConfigFileFinder, info.ConfigFileGenerator, info.Reporter);
            return info;
        }

        private class TestObjects
        {
            public TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions Options { get; set; }
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

            public string DefaultConfigFileName { get; private set; }

            public bool FindFile(string configFile)
            {
                return _shouldFindFile;
            }
        }

        private class TestCommandLineOptionHandlerConfigFileGenerator : IConfigFileGenerator
        {
            public readonly List<string> ConfigFilePathsWritten = new List<string>();

            public void WriteConfigFile(string path)
            {
                ConfigFilePathsWritten.Add(path);
            }
        }
    }
}