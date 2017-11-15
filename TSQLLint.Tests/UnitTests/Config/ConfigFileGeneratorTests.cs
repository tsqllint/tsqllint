using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.UnitTests.Config
{
    [TestFixture]
    public class ConfigFileGeneratorTests
    {
        private const string mockDirectory = @"c:\";

        [SetUp]
        public void Setup()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Assert.Ignore("Tests ignored on osx or linux until https://github.com/tathamoddie/System.IO.Abstractions/issues/252 is resolved");
            }
        }

        [Test]
        public void WriteConfigFile_FileDoesntExist_ShouldCreateFile()
        {
            // arrange
            var testFile = Path.Combine(mockDirectory, @".tsqllintrc");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(mockDirectory);
            var configFileGenerator = new ConfigFileGenerator(fileSystem);

            // act
            configFileGenerator.WriteConfigFile(testFile);

            // assert
            Assert.IsTrue(fileSystem.FileExists(testFile));
        }

        [Test]
        public void WriteConfigFile_FileExists_ShouldCreateFile()
        {
            // arrange
            var testFile = Path.Combine(mockDirectory, @".tsqllintrc");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { testFile, new MockFileData("{}") },
            });

            var configFileGenerator = new ConfigFileGenerator(fileSystem);

            // act
            configFileGenerator.WriteConfigFile(testFile);

            // assert
            Assert.IsTrue(fileSystem.FileExists(testFile));
        }

        [Test]
        public void GetDefaultConfigRules_ShouldReturnValidJson()
        {
            // arrange
            var configFileGenerator = new ConfigFileGenerator();

            // act
            var configString = configFileGenerator.GetDefaultConfigRules();

            // assert
            Assert.IsTrue(IsValidJson(configString));
        }

        [Test]
        public void GetDefaultConfigRules_ShouldReturnJsonConfigWithRules()
        {
            // arrange
            var configFileGenerator = new ConfigFileGenerator();

            // act
            var configString = configFileGenerator.GetDefaultConfigRules();
            ParsingUtility.TryParseJson(configString, out var configJson);

            // assert
            Assert.IsNotNull(configJson["rules"]);
        }

        [ExcludeFromCodeCoverage]
        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if (strInput.StartsWith("{") && strInput.EndsWith("}") || strInput.StartsWith("[") && strInput.EndsWith("]"))
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
