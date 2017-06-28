using NUnit.Framework;
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
            _configFilePath = Path.Combine(result + "\\IntegrationTests\\.tsqllintrc");

            return _configFilePath;
        }

        [Test]
        public void NoProblems()
        {
            var args = new[]
            {
                "-c", ConfigFilePath,
                "-p", "c:\\database\\foo.sql"
            };

            var noProblemsReporter = new NoProblemsReporter();
            noProblemsReporter.Report("test message");
            var commandLineOptions = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(args, noProblemsReporter);
            Assert.AreEqual(ConfigFilePath, commandLineOptions.ConfigFile);
            Assert.AreEqual("c:\\database\\foo.sql", commandLineOptions.LintPath);
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
        public void InvalidConfigFile()
        {
            var invalidConfigFileArgs = new[]
            {
                "-c", ".tsqllintrc-foo"
            };

            var invalidConfigFileReporter = new InvalidConfigFileReporter();
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(invalidConfigFileArgs, invalidConfigFileReporter);

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
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(emptyArgs, emptyArgsReporter);

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
            var initArgs = new[]
            {
                "-i"
            };
            var initArgsReporter = new InitArgsReporter();
            initArgsReporter.Report("test message");

            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(initArgs, initArgsReporter);

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
        public void NoLintPath()
        {
            var initArgs = new[]
            {
                "-c", ConfigFilePath,
                "-p", ""
            };

            var noLintPathReporter = new NoLintPathReporter();
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(initArgs, noLintPathReporter);

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
            var initArgs = new[]
            {
                "-c", ConfigFilePath,
                "-p", "foo.sql, bar.sql"
            };

            var lintPathFileListReporter = new LintPathFileListReporter();
            lintPathFileListReporter.Report("test message");

            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(initArgs, lintPathFileListReporter);

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