using System.Linq;
using NUnit.Framework;

namespace TSQLLINT_LIB_TESTS.UnitTests.CommandLineOptions
{
    public class CommandLineParserTest
    {
        [Test]
        public void Parses_Single_File()
        {
            // arrange
            const string Path = @"c:\database\foo.sql";

            var args = new[]
            {
                "-c", "config.file",
                Path
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual("config.file", commandLineOptions.ConfigFile);
            Assert.AreEqual(Path, commandLineOptions.LintPath.FirstOrDefault());
        }

        [Test]
        public void Parses_Multiple_Files()
        {
            // arrange
            const string FileOne = @"c:\database\foo.sql";
            const string FileTwo = @"c:\database\bar.sql";

            var args = new[]
            {
                FileOne,
                FileTwo
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(FileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(FileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void Parses_Multiple_Files_With_Spaces_In_Directory_Name()
        {
            // arrange
            const string FileOne = @"c:\database\foo.sql";
            const string FileTwo = @"c:\database directory\bar.sql";

            var args = new[]
            {
                FileOne,
                FileTwo
            };

            // act
            var commandLineOptions = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.AreEqual(FileOne, commandLineOptions.LintPath[0]);
            Assert.AreEqual(FileTwo, commandLineOptions.LintPath[1]);
        }

        [Test]
        public void Generates_Usage()
        {
            // arrange
            var args = new string[0];

            // act
            var commandLineParser = new TSQLLINT_CONSOLE.ConfigHandler.CommandLineOptions(args);

            // assert
            Assert.IsTrue(commandLineParser.GetUsage().Contains("tsqllint [options]"));
        }
    }
}