using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies;
using TSQLLint.Console.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Tests.UnitTests.CommandLineOptions.HandlingStrategies
{
    [TestFixture]
    public class CreateConfigFileStrategyTests
    {
        [SetUp]
        [ExcludeFromCodeCoverage]
        public void Setup()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Assert.Ignore("Tests ignored on osx or linux until https://github.com/tathamoddie/System.IO.Abstractions/issues/252 is resolved");
            }
        }

        [Test]
        public void HandleCommandLineOptions_ConfigFileExistsNoForceParam_ShouldReportError()
        {
            // arrange
            var mockReporter = Substitute.For<IBaseReporter>();
            mockReporter.Report(Arg.Any<string>());

            var mockConfigFileGenerator = Substitute.For<IConfigFileGenerator>();

            var mockCommandLineOptions = Substitute.For<ICommandLineOptions>();
            mockCommandLineOptions.Init = true;

            var mockFileSystem = new MockFileSystem();
            var configFilePath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
            mockFileSystem.AddFile(configFilePath, new MockFileData(string.Empty));

            var testCreateConfigFileStrategy = new CreateConfigFileStrategy(mockReporter, mockConfigFileGenerator, mockFileSystem);

            // act
            testCreateConfigFileStrategy.HandleCommandLineOptions(mockCommandLineOptions);

            // assert
            mockReporter.Received().Report(Arg.Is<string>(x => x.Contains("Default config file already exists at")));
        }

        [Test]
        public void HandleCommandLineOptions_ConfigFileExistsWithForceParam_ShouldCreateFile()
        {
            // arrange
            var mockReporter = Substitute.For<IBaseReporter>();
            mockReporter.Report(Arg.Any<string>());

            var mockConfigFileGenerator = Substitute.For<IConfigFileGenerator>();
            mockConfigFileGenerator.WriteConfigFile(Arg.Any<string>());

            var mockCommandLineOptions = Substitute.For<ICommandLineOptions>();
            mockCommandLineOptions.Init = true;
            mockCommandLineOptions.Force = true;

            var mockFileSystem = new MockFileSystem();
            var configFilePath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
            mockFileSystem.AddFile(configFilePath, new MockFileData(string.Empty));

            var testCreateConfigFileStrategy = new CreateConfigFileStrategy(mockReporter, mockConfigFileGenerator, mockFileSystem);

            // act
            testCreateConfigFileStrategy.HandleCommandLineOptions(mockCommandLineOptions);

            // assert
            mockConfigFileGenerator.Received().WriteConfigFile(Arg.Is<string>(x => x == configFilePath));
        }

        [Test]
        public void HandleCommandLineOptions_ConfigFileDoesntExistWithForceParam_ShouldCreateFile()
        {
            // arrange
            var mockReporter = Substitute.For<IBaseReporter>();
            mockReporter.Report(Arg.Any<string>());

            var mockConfigFileGenerator = Substitute.For<IConfigFileGenerator>();
            mockConfigFileGenerator.WriteConfigFile(Arg.Any<string>());

            var mockCommandLineOptions = Substitute.For<ICommandLineOptions>();
            mockCommandLineOptions.Init = true;
            mockCommandLineOptions.Force = true;

            var mockFileSystem = new MockFileSystem();
            var configFilePath = mockFileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");

            var testCreateConfigFileStrategy = new CreateConfigFileStrategy(mockReporter, mockConfigFileGenerator, mockFileSystem);

            // act
            testCreateConfigFileStrategy.HandleCommandLineOptions(mockCommandLineOptions);

            // assert
            mockConfigFileGenerator.Received().WriteConfigFile(Arg.Is<string>(x => x == configFilePath));
        }
    }
}
