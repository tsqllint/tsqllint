using NUnit.Framework;
using System;
using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.CommandLineParser
{
    class CommandLineParserTest
    {
        private string _configFilePath;
        private string ConfigFilePath
        {
            get { return (string.IsNullOrWhiteSpace(_configFilePath) == false) ? _configFilePath : InitializeConfigFilePath(); }
        }

        private string InitializeConfigFilePath()
        {
            var testDirectoryInfo = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            var result = testDirectoryInfo.Parent.Parent.FullName;
            _configFilePath = Path.Combine(result + @"\IntegrationTests\.tsqllintrc");

            return _configFilePath;
        }

        [Test]
        public void NoProblems()
        {
            // arrange
            var args = new[]
            {
                "-c", ConfigFilePath,
                "-f", @"c:\database\foo.sql"
            };

            var noProblemsReporter = new NoProblemsReporter();
            noProblemsReporter.Report("test message");

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(args, noProblemsReporter);

            // assert
            Assert.AreEqual(ConfigFilePath, commandLineOptions.ConfigFile);
            Assert.AreEqual(@"c:\database\foo.sql", commandLineOptions.LintPath);
            Assert.AreEqual(1, noProblemsReporter.MessageCount);
            Assert.AreEqual(true, commandLineOptions.PerformLinting);
        }

        private class NoProblemsReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
            }
        }

        [Test]
        public void DefaultConfigFile()
        {
            // arrange
            var invalidConfigFileArgs = new string[0];

            var initArgsReporter = new InitArgsReporter();
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(invalidConfigFileArgs, initArgsReporter);

            // act
            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            //assert

            // should default to config file in user directory
            Assert.AreEqual(Path.Combine(usersDirectory, @".tsqllintrc"), commandLineParser.ConfigFile);

            // should not continue with linting
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        [Test]
        public void InvalidConfigFile()
        {
            // arrange
            var invalidConfigFileArgs = new[]
            {
                "-c", ".tsqllintrc-foo"
            };

            var invalidConfigFileReporter = new InvalidConfigFileReporter();

            // act
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(invalidConfigFileArgs, invalidConfigFileReporter);

            // assert
            Assert.AreEqual(2, invalidConfigFileReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        private class InvalidConfigFileReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;

                // the first message should be an error message about the config file
                if (MessageCount == 1)
                {
                    Assert.AreEqual("Config file not found .tsqllintrc-foo.", message);
                }

                // the second message should contain usage information
                if (MessageCount == 2)
                {
                    Assert.IsTrue(message.Contains("Usage: tsqllint [options]"));
                }
            }
        }

        [Test]
        public void EmptyArgs()
        {
            var emptyArgs = new string[0];

            var emptyArgsReporter = new EmptyArgsReporter();
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(emptyArgs, emptyArgsReporter);

            Assert.AreEqual(1, emptyArgsReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        private class EmptyArgsReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
                Assert.IsTrue(message.Contains("Usage: tsqllint [options]"));
            }
        }

        [Test]
        public void InitArgs()
        {
            // arrange

            var initArgs = new[]
            {
                "-i"
            };
            var initArgsReporter = new InitArgsReporter();
            initArgsReporter.Report("test message");

            // act

            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(initArgs, initArgsReporter);

            // assert

            // only the test message should have been sent
            Assert.AreEqual(1, initArgsReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        private class InitArgsReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
            }
        }

        [Test]
        public void VersionArgs()
        {
            // arrange

            var initArgs = new[]
            {
                "-v"
            };
            var versionArgsReporter = new VersionArgsReporter();
            versionArgsReporter.Report("test message");

            // act
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(initArgs, versionArgsReporter);

            // assert

            // only the test message should have been sent
            Assert.AreEqual(1, versionArgsReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        [Test]
        public void PrintConfigArgs()
        {
            // arrange

            var printConfigArgs = new[]
            {
                "-p"
            };
            var versionArgsReporter = new VersionArgsReporter();
            versionArgsReporter.Report("test message");

            // act
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(printConfigArgs, versionArgsReporter);

            // assert

            // only the test message should have been sent
            Assert.AreEqual(1, versionArgsReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        private class VersionArgsReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
            }
        }

        [Test]
        public void NoLintPath()
        {
            // arrange
            var initArgs = new[]
            {
                "-c", ConfigFilePath,
                "-f", ""
            };

            var noLintPathReporter = new NoLintPathReporter();

            // act
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(initArgs, noLintPathReporter);

            // assert

            // only the test message should have been sent
            Assert.AreEqual(1, noLintPathReporter.MessageCount);
            Assert.AreEqual(false, commandLineParser.PerformLinting);
        }

        private class NoLintPathReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
                Assert.IsTrue(message.Contains("Usage: tsqllint [options]"));
            }
        }


        [Test]
        public void LintPathFileList()
        {
            // arrange

            var initArgs = new[]
            {
                "-c", ConfigFilePath,
                "-f", "foo.sql, bar.sql"
            };

            var lintPathFileListReporter = new LintPathFileListReporter();
            lintPathFileListReporter.Report("test message");

            // act

            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.ConsoleCommandLineOptionParser(initArgs, lintPathFileListReporter);

            // assert

            Assert.AreEqual(1, lintPathFileListReporter.MessageCount);
            Assert.AreEqual(true, commandLineParser.PerformLinting);
        }

        private class LintPathFileListReporter : IBaseReporter
        {
            public int MessageCount;

            public void Report(string message)
            {
                MessageCount++;
            }
        }
    }
}