using NUnit.Framework;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.CommandLineParser
{
    class CommandLineParserTest
    {
        [Test]
        public void NoProblems()
        {
            var args = new[]
            {
                "-p", "c:\\database\\foo.sql"
            };

            var noProblemsReporter = new NoProblemsReporter();
            noProblemsReporter.Report("test message");
            var commandLineOptions = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(args, noProblemsReporter);
            Assert.AreEqual(".tsqllintrc", commandLineOptions.ConfigFile);
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
                    Assert.IsTrue(message.Contains("Usage: TSQLLINT [options]"));
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

                // the second message should contain usage information
                if (MessageCount == 1)
                {
                    Assert.IsTrue(message.Contains("Usage: TSQLLINT [options]"));
                }
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

                if (MessageCount == 1)
                {
                    Assert.IsTrue(message.Contains("Usage: TSQLLINT [options]"));
                }
            }
        }
    }
}