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
        public void IgnoreListReaderExplicitPathThatExists_LoadsFromThatFile()
        {
            // arrange: an explicit path is supplied and the file exists
            var explicitPath = TestHelper.GetTestFilePath(@"c:\configs\.tsqllintignore");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    explicitPath, new MockFileData(@"
                        test1.sql
                        test2.sql
                    ")
                }
            });
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(explicitPath);

            // assert
            Assert.That(ignoreListReader.IsIgnoreListLoaded, Is.True);
            Assert.That(ignoreListReader.IgnoreList, Is.EqualTo(new List<string> { "test1.sql", "test2.sql" }));
            reporter.DidNotReceive().Report(Arg.Any<string>());
        }

        [Test]
        public void IgnoreListReaderEnvironmentVariablePointsToMissingFile_FallsBackToEmpty()
        {
            // arrange: env var is set but the file it points to does not exist,
            // and there is no local or user-profile ignore file either.
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();
            environmentWrapper.GetEnvironmentVariable("tsqllintignore")
                .Returns(TestHelper.GetTestFilePath(@"c:\does\not\exist\.tsqllintignore"));

            // act
            var ignoreListReader = new IgnoreListReader(reporter, fileSystem, environmentWrapper);
            ignoreListReader.LoadIgnoreList(null);

            // assert: it fell past the missing env-var file to the empty default
            Assert.That(ignoreListReader.IsIgnoreListLoaded, Is.True);
            Assert.That(ignoreListReader.IgnoreList, Is.EqualTo(new List<string>()));
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
