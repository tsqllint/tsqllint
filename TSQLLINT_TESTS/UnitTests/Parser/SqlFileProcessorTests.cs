using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
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
        public void ProcessFileProcessesRulesForContentAndIncrementsFileCount()
        {
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);

            const string FileContents = "MyFileContents";
            const string FilePath = "PathToFile.sql";

            processor.ProcessFile(FileContents, FilePath);

            ruleVisitor.Received().VisitRules(FilePath, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(1, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesSingleFileWhenItExists()
        {
            const string FilePath = "c:\\dbscripts\\myfile.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            fileBase.Exists(FilePath).Returns(true);
            fileBase.ReadAllText(FilePath).Returns("Some Sql To Parse");
            fileSystem.File.Returns(fileBase);

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath("\" " + FilePath + " \""); // Also testing removal of quotes and leading/trailing spaces

            fileBase.Received().Exists(FilePath);
            fileBase.Received().ReadAllText(FilePath);
            ruleVisitor.Received().VisitRules(FilePath, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(1, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesDirectoryOfOnlyOfMixedFiles()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\file2.txt";
            const string FilePath3 = @"c:\dbscripts\file3.sql";
            const string FilePath4 = @"c:\dbscripts\file4.Sql";
            
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath("\" " + @"c:\DBScripts" + " \""); // Also testing removal of quotes and leading/trailing spaces

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesDirectoryDirectories()
        {
            const string FilePath1 = @"c:\dbscripts\db1\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\db1\file2.sQL";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts");

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath2, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(2, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesDirectoryOfFilesAndDirectories()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\db1\file2.sql";
            const string FilePath3 = @"c:\dbscripts\db2\file3.sql";
            const string FilePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts");

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(4, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathDoesNotProcessWhenNotFileDirectoryOrWildcard()
        {
            const string FilePath = "This doesnt exist";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            fileBase.Exists(FilePath).Returns(false);
            fileSystem.File.Returns(fileBase);
            var directoryBase = Substitute.For<DirectoryBase>();
            directoryBase.Exists(FilePath).Returns(false);
            fileSystem.Directory.Returns(directoryBase);

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(FilePath);

            fileBase.Received().Exists(FilePath);
            directoryBase.Received().Exists(FilePath);
            ruleVisitor.DidNotReceive().VisitRules(FilePath, Arg.Any<TextReader>());
            reporter.Received().Report(string.Format("{0} is not a valid path.", FilePath));
            Assert.AreEqual(0, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesWildCardWithQuestionMark()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\file2.txt";
            const string FilePath3 = @"c:\dbscripts\file3.sql";
            const string FilePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\file?.sql");

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());            
        }

        [Test]
        public void ProcessPathProcessesWildCardWithAsterix()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\file2.txt";
            const string FilePath3 = @"c:\dbscripts\file3.sql";
            const string FilePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\file*.*");

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathProcessesWildCardWhenItNeedsToUseCurrentDirectoryPath()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\file2.txt";
            const string FilePath3 = @"c:\dbscripts\file3.sql";
            const string FilePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            }, 
            @"c:\dbscripts"); // Set current directory

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"file*.*");

            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());

            reporter.DidNotReceive().Report(Arg.Any<string>());

            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPathDoesNotProcessFilesWhenWildCardDoesNotFindAnything()
        {
            const string FilePath1 = @"c:\dbscripts\file1.txt";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\oops*.*");

            ruleVisitor.DidNotReceive().VisitRules(FilePath1, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.GetFileCount());
        }

        [Test]
        public void ProcessListDoesNotProcessAnyFilesForEmptyList()
        {
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessList(new List<string>());

            ruleVisitor.DidNotReceive().VisitRules(Arg.Any<string>(), Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.GetFileCount());
        }

        [Test]
        public void ProcessListProcessesListOfItems()
        {
            const string FilePath1 = @"c:\dbscripts\file1.SQL";
            const string FilePath2 = @"c:\dbscripts\db1\file2.sql";
            const string FilePath3 = @"c:\dbscripts\db2\file3.sql";
            const string FilePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
                {FilePath3, new MockFileData("File3SQL")},
                {FilePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);

            // handles quotes, extra spaces, commas, multiple items in the list
            processor.ProcessList(new List<string> { "\" c:\\dbscripts\\db2\\sproc , c:\\dbscripts\\db2\\file3.sql \"", @"c:\dbscripts\db1\" });

            ruleVisitor.DidNotReceive().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());            
        }

        [Test]
        public void ShouldNotThrowWhenPassedNonExistantPath()
        {
            const string FilePath1 = @"c:\dbscripts\db1\file2.sql";
            const string FilePath2 = @"c:\dbscripts\db1\file3.sql";

            const string InvalidFilePath = @"c:\invalid\invalid.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {FilePath1, new MockFileData("File1SQL")},
                {FilePath2, new MockFileData("File2SQL")},
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);

            processor.ProcessList(new List<string> { InvalidFilePath, @"c:\dbscripts\db1\" });

            ruleVisitor.DidNotReceive().VisitRules(InvalidFilePath, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(FilePath2, Arg.Any<TextReader>());

            reporter.Received().Report(@"Directory doest not exit: c:\invalid");

            Assert.AreEqual(2, processor.GetFileCount());
        }
    }
}
