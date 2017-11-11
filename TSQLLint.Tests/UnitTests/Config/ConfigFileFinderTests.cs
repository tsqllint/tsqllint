using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;
using TSQLLint.Lib.Config;

namespace TSQLLint.Tests.UnitTests.Config
{
    public class ConfigFileFinderTests
    {
        private const string mockDirectory = @"c:\";
        
        [Test]
        public void FindFile_FileExists_ShouldFindFile()
        {
            // arrange
            var testFile = Path.Combine(mockDirectory, @".tsqllintrc");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { testFile, new MockFileData("{}") },
            });

            // act
            var configFileFinder = new ConfigFileFinder(fileSystem);

            // assert
            Assert.IsTrue(configFileFinder.FindFile(testFile));
        }

        [Test]
        public void FindFile_FileDoesntExist_ShouldFindFile()
        {
            // arrange
            var testFile = Path.Combine(mockDirectory, @".tsqllintrc");
            var fileSystem = new MockFileSystem();

            // act
            var configFileFinder = new ConfigFileFinder(fileSystem);

            // assert
            Assert.IsFalse(configFileFinder.FindFile(testFile));
        }
    }
}
