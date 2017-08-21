using System.IO;
using System.IO.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.UnitTests.Parser
{
    [TestFixture]
    public class SqlFileProcessorTests
    {
        [Test]
        public void ProcessFile_Processes_Rules_For_Content_And_Increments_FileCount()
        {
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);

            const string fileContents = "MyFileContents";
            const string filePath = "PathToFile.sql";

            processor.ProcessFile(fileContents, filePath);

            ruleVisitor.Received().VisitRules(filePath, Arg.Any<TextReader>());
            Assert.AreEqual(1, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_Single_File_When_It_Exists()
        {
            const string filePath = "c:\\dbscripts\\myfile.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            fileBase.Exists(filePath).Returns(true);
            fileBase.ReadAllText(filePath).Returns("Some Sql To Parse");
            fileSystem.File.Returns(fileBase);

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath("\" " + filePath + " \""); // Also testing removal of quotes and leading/trailing spaces

            fileBase.Received().Exists(filePath);
            fileBase.Received().ReadAllText(filePath);
            ruleVisitor.Received().VisitRules(filePath, Arg.Any<TextReader>());
            Assert.AreEqual(1, processor.GetFileCount());
        }

        //TODO ProcessPath Directory of not all SQL files
        //TODO ProcessPath Directory with Directories
        //TODO ProcessPath with file / directory mixed
        //TODO ProcessPath Not Directory/File/Wildcard
        //TODO ProcessPath Wildcard with ? (no directory)
        //TODO ProcessPath Wildcard with * (no directory)
        //TODO ProcessPath Wildcard with * its own Directory
        //TODO ProcessPath Wildcard nothing found
        //TODO ProcessList with no items
        //TODO ProcessList with items
    }
}
