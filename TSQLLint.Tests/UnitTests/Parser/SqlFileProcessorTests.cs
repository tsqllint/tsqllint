using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.Reporters;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins.Interfaces;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class SqlFileProcessorTests
    {
        [Test]
        public void ProcessFileProcessesRulesForContentAndIncrementsFileCount()
        {
            // arrange
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();

            var pluginHandler = Substitute.For<IPluginHandler>();

            pluginHandler.Plugins.Returns(
                new List<IPlugin>
                {
                    Substitute.For<IPlugin>()
                });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            const string fileContents = "MyFileContents";
            const string filePath = "PathToFile.sql";

            // act
            processor.ProcessFile(ParsingUtility.GenerateStreamFromString(fileContents), filePath);

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(1, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesSingleFileWhenItExists()
        {
            // arrange
            const string filePath = "c:\\dbscripts\\myfile.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            fileBase.Exists(filePath).Returns(true);
            fileBase.OpenRead(filePath).Returns(ParsingUtility.GenerateStreamFromString("Some Sql To Parse"));
            fileSystem.File.Returns(fileBase);

            // act
            processor.ProcessPath("\" " + filePath + " \""); // Also testing removal of quotes and leading/trailing spaces

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            fileBase.Received().Exists(filePath);
            fileBase.Received().OpenRead(filePath);
            ruleVisitor.Received().VisitRules(filePath, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(1, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesDirectoryOfOnlyOfMixedFiles()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";
            
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var reporter = Substitute.For<IReporter>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                },
                {
                    filePath3, new MockFileData("File3SQL")
                },
                {
                    filePath4, new MockFileData("File4SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath("\" " + @"c:\DBScripts" + " \""); // Also testing removal of quotes and leading/trailing spaces

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesDirectoryDirectories()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\db1\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sQL";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"c:\DBScripts");

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(2, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesDirectoryOfFilesAndDirectories()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sql";
            const string filePath3 = @"c:\dbscripts\db2\file3.sql";
            const string filePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                },
                {
                    filePath3, new MockFileData("File3SQL")
                },
                {
                    filePath4, new MockFileData("File4SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"c:\DBScripts");

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(4, processor.FileCount);
        }

        [Test]
        public void ProcessPathDoesNotProcessWhenNotFileDirectoryOrWildcard()
        {
            // arrange
            const string filePath = "This doesnt exist";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            fileBase.Exists(filePath).Returns(false);
            fileSystem.File.Returns(fileBase);
            var directoryBase = Substitute.For<DirectoryBase>();
            directoryBase.Exists(filePath).Returns(false);
            fileSystem.Directory.Returns(directoryBase);

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(filePath);

            // assert
            pluginHandler.DidNotReceive().ActivatePlugins(Arg.Any<IPluginContext>());
            fileBase.Received().Exists(filePath);
            directoryBase.Received().Exists(filePath);
            ruleVisitor.DidNotReceive().VisitRules(filePath, Arg.Any<Stream>());
            reporter.Received().Report($"{filePath} is not a valid path.");
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesWildCardWithQuestionMark()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                },
                {
                    filePath3, new MockFileData("File3SQL")
                },
                {
                    filePath4, new MockFileData("File4SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"c:\DBScripts\file?.sql");

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.FileCount);            
        }

        [Test]
        public void ProcessPathProcessesWildCardWithAsterix()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                },
                {
                    filePath3, new MockFileData("File3SQL")
                },
                {
                    filePath4, new MockFileData("File4SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"c:\DBScripts\file*.*");

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPathProcessesWildCardWhenItNeedsToUseCurrentDirectoryPath()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\file2.txt";
            const string filePath3 = @"c:\dbscripts\file3.sql";
            const string filePath4 = @"c:\dbscripts\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        filePath1, new MockFileData("File1SQL")
                    },
                    {
                        filePath2, new MockFileData("File2SQL")
                    },
                    {
                        filePath3, new MockFileData("File3SQL")
                    },
                    {
                        filePath4, new MockFileData("File4SQL")
                    }
                }, 
                @"c:\dbscripts");

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"file*.*");

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());

            reporter.DidNotReceive().Report(Arg.Any<string>());

            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPathDoesNotProcessFilesWhenWildCardDoesNotFindAnything()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.txt";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessPath(@"c:\DBScripts\oops*.*");

            // assert
            pluginHandler.DidNotReceive().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessListDoesNotProcessAnyFilesForEmptyList()
        {
            // arrange
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessList(new List<string>());

            // assert
            pluginHandler.DidNotReceive().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.DidNotReceive().VisitRules(Arg.Any<string>(), Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessListProcessesListOfItems()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\file1.SQL";
            const string filePath2 = @"c:\dbscripts\db1\file2.sql";
            const string filePath3 = @"c:\dbscripts\db2\file3.sql";
            const string filePath4 = @"c:\dbscripts\db2\sproc\file4.Sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                },
                {
                    filePath3, new MockFileData("File3SQL")
                },
                {
                    filePath4, new MockFileData("File4SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessList(new List<string> { "\" c:\\dbscripts\\db2\\sproc , c:\\dbscripts\\db2\\file3.sql \"", @"c:\dbscripts\db1\" });             // tests quotes, extra spaces, commas, multiple items in the list

            // assert
            pluginHandler.Received().ActivatePlugins(Arg.Any<IPluginContext>());
            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(3, processor.FileCount);            
        }

        [Test]
        public void ProcessList_PassedInvalidPaths_ShouldNotThrow()
        {
            // arrange
            const string filePath1 = @"c:\dbscripts\db1\file2.sql";
            const string filePath2 = @"c:\dbscripts\db1\file3.sql";
            const string invalidFilePath = @"c:\invalid\invalid.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem);

            // act
            processor.ProcessList(new List<string> { invalidFilePath, @"c:\dbscripts\db1\" });

            // assert
            ruleVisitor.DidNotReceive().VisitRules(invalidFilePath, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<Stream>());
            reporter.Received().Report($@"{invalidFilePath} is not a valid path.");
            Assert.AreEqual(2, processor.FileCount);
        }
    }
}
