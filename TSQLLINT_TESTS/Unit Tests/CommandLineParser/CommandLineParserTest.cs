using NUnit.Framework;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.CommandLineParser
{
    class CommandLineParserTest
    {
        [TestCase(new[]
        {
            "-p", "c:\\database\\foo.sql",
        }, 2)]

        public void CommaneLineParserTest(string[] args, int violationCount)
        {
            var commandLineParser = new TSQLLINT_CONSOLE.CommandLineParser.CommandLineParser(args);
            var commaneLineArgs = commandLineParser.GetCommandLineOptions();
            Assert.AreEqual(".tsqllintrc", commaneLineArgs.ConfigFile);
            Assert.AreEqual("c:\\database\\foo.sql", commaneLineArgs.LintPath);
        }
    }
}