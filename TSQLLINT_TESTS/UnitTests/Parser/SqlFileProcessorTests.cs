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
            reporter.DidNotReceive().Report(Arg.Any<string>());
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
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(1, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_Directory_Of_Only_Of_Mixed_Files()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";
            
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath("\" " + @"c:\DBScripts" + " \""); // Also testing removal of quotes and leading/trailing spaces

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_Directory_Directories()
        {
            const string filePath1 = @"c:\dbscripts\db1\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sQL";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts");

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(2, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_Directory_Of_Files_And_Directories()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sql";
            const string filePath3 = @"c:\dbscripts\db2\file3.sql";
            const string filePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts");

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(4, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Does_Not_Process_When_Not_File_Directory_Or_Wildcard()
        {
            const string filePath = "This doesnt exist";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            fileBase.Exists(filePath).Returns(false);
            fileSystem.File.Returns(fileBase);
            var directoryBase = Substitute.For<DirectoryBase>();
            directoryBase.Exists(filePath).Returns(false);
            fileSystem.Directory.Returns(directoryBase);

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(filePath);

            fileBase.Received().Exists(filePath);
            directoryBase.Received().Exists(filePath);
            ruleVisitor.DidNotReceive().VisitRules(filePath, Arg.Any<TextReader>());
            reporter.Received().Report(string.Format("{0} is not a valid path.", filePath));
            Assert.AreEqual(0, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_WildCard_With_QuestionMark()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\file?.sql");

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());            
        }

        [Test]
        public void ProcessPath_Processes_WildCard_With_Asterix()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\file*.*");

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Processes_WildCard_When_It_Needs_To_Use_CurrentDirectory_Path()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            }, @"c:\dbscripts"); // Set current directory

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"file*.*");

            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());

            reporter.DidNotReceive().Report(Arg.Any<string>());

            Assert.AreEqual(3, processor.GetFileCount());
        }

        [Test]
        public void ProcessPath_Does_Not_Process_Files_When_WildCard_Does_Not_Find_Anything()
        {
            const string filePath1 = @"c:\dbscripts\file1.txt";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            processor.ProcessPath(@"c:\DBScripts\oops*.*");

            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.GetFileCount());
        }

        [Test]
        public void ProcessList_Does_Not_Process_Any_Files_For_Empty_List()
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
        public void ProcessList_Processes_List_Of_Items()
        {
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sql";
            const string filePath3 = @"c:\dbscripts\db2\file3.sql";
            const string filePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
                {filePath3, new MockFileData("File3SQL")},
                {filePath4, new MockFileData("File4SQL")}
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);
            // handles quotes, extra spaces, commas, multiple items in the list
            processor.ProcessList(new List<string> { "\" c:\\dbscripts\\db2\\sproc , c:\\dbscripts\\db2\\file3.sql \"", @"c:\dbscripts\db1\" });

            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<TextReader>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.GetFileCount());            
        }

        [Test]
        public void Should_Not_Throw_When_Passed_NonExistant_Path()
        {
            const string filePath1 = @"c:\dbscripts\db1\file2.sql";
            const string filePath2 = @"c:\dbscripts\db1\file3.sql";

            const string invalidFilePath = @"c:\invalid\invalid.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IBaseReporter>();
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData("File1SQL")},
                {filePath2, new MockFileData("File2SQL")},
            });

            var processor = new SqlFileProcessor(ruleVisitor, reporter, fileSystem);

            processor.ProcessList(new List<string> { invalidFilePath, @"c:\dbscripts\db1\" });

            ruleVisitor.DidNotReceive().VisitRules(invalidFilePath, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<TextReader>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<TextReader>());

            reporter.Received().Report(@"Directory does not exit: c:\invalid");

            Assert.AreEqual(2, processor.GetFileCount());
        }
    }
}
