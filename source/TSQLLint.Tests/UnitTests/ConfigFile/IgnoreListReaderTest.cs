using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Tests.Helpers;
using static System.String;

namespace TSQLLint.Tests.UnitTests.ConfigFile
{
    [TestFixture]
    public class IgnoreListReaderTest
    {
        [Test]
        public void IgnoreListReaderEmptyPath()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(Empty);

            // assert
            Assert.IsTrue(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(ignoreListReader.IgnoreList, new List<string>());
        }

        [Test]
        public void IgnoreListReaderInMemoryList()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(Empty);

            // assert
            Assert.IsTrue(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(ignoreListReader.IgnoreList, new List<string>());
        }

        [Test]
        public void IgnoreListReaderFileDoesntExist()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintignore"));

            // assert
            Assert.IsFalse(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(ignoreListReader.IgnoreList, new List<string>());
        }

        [Test]
        public void IgnoreListReaderFromUserProfile()
        {
            // arrange
            var ignoreListFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintignore");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    ignoreListFilePath, new MockFileData(@"
                        test1.sql
                        test2.sql
                    ")
                }
            });

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(null);

            // assert
            Assert.IsTrue(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(ignoreListReader.IgnoreList, new List<string> { "test1.sql", "test2.sql" });
        }

        [Test]
        public void IgnoreListReaderFromLocal()
        {
            // arrange
            var localConfigFile = Path.Combine(TestContext.CurrentContext.TestDirectory, ".tsqllintignore");
            var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                {
                    // should ignore config files in user profile when local config exists
                    TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintignore"), new MockFileData(@"
                        test1.sql
                        test2.sql
                    ")
                },
                {
                    localConfigFile, new MockFileData(@"
                        test3.sql
                        test4.sql
                ")
                }
            }, TestContext.CurrentContext.TestDirectory);

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(null);

            // assert
            Assert.IsTrue(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(ignoreListReader.IgnoreList, new List<string> { "test3.sql", "test4.sql" });
        }

        [Test]
        public void ConfigReaderLoadsConfigsEnvironmentVariable()
        {
            // arrange
            var testConfigFile = TestHelper.GetTestFilePath(@"c:\foo\.tsqllintignore");

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        // should ignore config files in user profile when local config exists
                        testConfigFile, new MockFileData(@"
                            test1.sql
                            test2.sql
                        ")
                    },
                    {
                        // should ignore config files in user profile when local config exists
                        TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintignore"), new MockFileData(@"
                            test3.sql
                            test4.sql
                        ")
                    },
                }, TestContext.CurrentContext.TestDirectory);

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();
            environmentWrapper.GetEnvironmentVariable("tsqllintignore").Returns(testConfigFile);

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(null);

            // assert
            Assert.IsTrue(ignoreListReader.IsIgnoreListLoaded);
            CollectionAssert.AreEqual(new List<string> { "test1.sql", "test2.sql" }, ignoreListReader.IgnoreList);
        }
    }
}
