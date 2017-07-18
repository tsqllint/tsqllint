using NUnit.Framework;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.CommandLineParser
{
    class CommandLineParserTest
    {
        [TestCase(new[]
        {
            "-p", "c:\\database\\foo.sql"
        }, 1)]
        public void CommaneLineParser(string[] args, int value)
        {
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(args, null);
            var commaneLineArgs = commandLineParser.GetCommandLineOptions();
            Assert.AreEqual(".tsqllintrc", commaneLineArgs.ConfigFile);
            Assert.AreEqual("c:\\database\\foo.sql", commaneLineArgs.LintPath);
        }

        [Test]
        public void GetUsage()
        {
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(null, null);
            Assert.IsNotNull(commandLineParser.GetUsage());
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
        }

        private class BadArgsReporter : IBaseReporter
        {
            private int messageCount;

            public void Report(string message)
            {
                messageCount++;

                // the first message should be an error message about the config file
                if (messageCount == 1)
                {
                    Assert.AreEqual("Config file not found .tsqllintrc-foo.", message);
                }

                // the second message should contain usage information
                if (messageCount == 2)
                {
                    Assert.IsTrue(message.Contains("Usage: TSQLLINT [options]"));
                }
            }
        }
    }
}