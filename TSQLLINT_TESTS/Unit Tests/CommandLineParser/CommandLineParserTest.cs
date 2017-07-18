using NUnit.Framework;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.CommandLineParser
{
    class CommandLineParserTest
    {
        [Test]
        public void CommaneLineParser()
        {
            var args = new[]
            {
                "-p", "c:\\database\\foo.sql"
            };

            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(args, null);
            var commaneLineArgs = commandLineParser.GetCommandLineOptions();
            Assert.AreEqual(".tsqllintrc", commaneLineArgs.ConfigFile);
            Assert.AreEqual("c:\\database\\foo.sql", commaneLineArgs.LintPath);
        }

        [Test]
        public void BadArguments()
        {
            var badArgs = new[]
            {
                "-c", ".tsqllintrc-foo"
            };

            var badArgsReporter = new BadArgsReporter();
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(badArgs, badArgsReporter);
            commandLineParser.GetCommandLineOptions();

            Assert.AreEqual(2, BadArgsReporter.MessageCount);
        }

        private class BadArgsReporter : IBaseReporter
        {
            public static int MessageCount;

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
        public void GetUsage()
        {
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(null, null);
            Assert.IsNotNull(commandLineParser.GetUsage());
        }
    }
}