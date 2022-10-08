using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class SqlFileProcessorTests
    {
        private static Dictionary<string, Type> ruleList = RuleVisitorFriendlyNameTypeMap.List;

        [Test]
        public void ProcessPath_SingleFile_ShouldProcessFile()
        {
            // arrange
            const string filePath = "c:\\dbscripts\\myfile.sql";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();
            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            fileBase.Exists(filePath).Returns(true);
            fileBase.OpenRead(filePath).Returns(ParsingUtility.GenerateStreamFromString("Some Sql To Parse"));
            fileSystem.File.Returns(fileBase);

            // act
            processor.ProcessPath("\" " + filePath + " \""); // Also testing removal of quotes and leading/trailing spaces

            // assert
            fileBase.Received().Exists(filePath);
            fileBase.Received().OpenRead(filePath);
            ruleVisitor.Received().VisitRules(filePath, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(1, processor.FileCount);
        }

        [Test]
        public void ProcessPath_PathWithSpaces_ShouldProcessFiles()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\file2.txt");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\file3.sql");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\file4.Sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var reporter = Substitute.For<IReporter>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

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

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);
            globPatternMatcher.GetResultsInFullPath(TestHelper.GetTestFilePath(@"c:\dbscripts")).Returns(new[] { filePath1, filePath3, filePath4 });

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\dbscripts"));

            // assert
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<IEnumerable<IRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPath_DirectorySpecified_ShouldProcessSubDirectories()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\db1\file2.sql");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\file3.sql");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\sproc\file4.Sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

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

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);
            globPatternMatcher.GetResultsInFullPath(TestHelper.GetTestFilePath(@"c:\dbscripts")).Returns(new[] { filePath1, filePath2, filePath3, filePath4 });

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\dbscripts"));

            // assert
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(4, processor.FileCount);
        }

        [Test]
        public void ProcessPath_InvalidPathSpecified_ShouldNotProcessFiles()
        {
            // arrange
            const string filePath = "This doesnt exist";

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var fileBase = Substitute.For<FileBase>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            fileBase.Exists(filePath).Returns(false);
            fileSystem.File.Returns(fileBase);
            var directoryBase = Substitute.For<DirectoryBase>();
            directoryBase.Exists(filePath).Returns(false);
            fileSystem.Directory.Returns(directoryBase);

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(filePath);

            // assert
            fileBase.Received().Exists(filePath);
            directoryBase.Received().Exists(filePath);
            ruleVisitor.DidNotReceive().VisitRules(filePath, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            reporter.Received().Report($"{filePath} is not a valid file path.");
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessPath_QuestionMarkWildCard_ShouldProcessFilesWithWildcard()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\file2.txt");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\file3.sql");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\file4.Sql");
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

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

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\dbscripts\file?.sql"));

            // assert
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPath_DirectorSpecifiedWildcard_ShouldOnlyProcessSqlFilesInSpecificDirectory()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\file2.txt");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\file3.sql");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\file4.Sql");
            var filePath5 = TestHelper.GetTestFilePath(@"c:\file4.Sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

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
                },
                {
                    filePath5, new MockFileData("File5SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\dbscripts\file*.*"));

            // assert
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath2, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.DidNotReceive().VisitRules(filePath5, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPath_WildcardInvalidDirectory_ShouldNotProcess()
        {
            // arrange
            var sqlFilePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        sqlFilePath1, new MockFileData("File1SQL")
                    }
                },
                TestHelper.GetTestFilePath(@"c:\dbscripts"));

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\doesntExist\file*.*"));

            // assert
            ruleVisitor.DidNotReceive().VisitRules(sqlFilePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());

            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessPath_Wildcard_ShouldOnlyProcessSqlFilesInCurrentDirectory()
        {
            // arrange
            var sqlFilePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var txtFilePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\file2.txt");
            var sqlFilePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\file3.sql");
            var sqlFilePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\file4.Sql");
            var sqlFilePath5 = TestHelper.GetTestFilePath(@"c:\file4.Sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        sqlFilePath1, new MockFileData("File1SQL")
                    },
                    {
                        txtFilePath2, new MockFileData("File2SQL")
                    },
                    {
                        sqlFilePath3, new MockFileData("File3SQL")
                    },
                    {
                        sqlFilePath4, new MockFileData("File4SQL")
                    },
                    {
                        sqlFilePath5, new MockFileData("File5SQL")
                    }
                }, TestHelper.GetTestFilePath(@"c:\dbscripts"));

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(@"file*.*");

            // assert
            ruleVisitor.Received().VisitRules(sqlFilePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(sqlFilePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(sqlFilePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());

            // should not visit text files
            ruleVisitor.DidNotReceive().VisitRules(txtFilePath2, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());

            // should only visit files in current directory
            ruleVisitor.DidNotReceive().VisitRules(sqlFilePath5, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());

            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessPath_InvalidPath_ShouldNotProcess()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.txt");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessPath(TestHelper.GetTestFilePath(@"c:\dbscripts\invalid*.*"));

            // assert
            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            reporter.DidNotReceive().Report(Arg.Any<string>());
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessList_EmptyList_ShouldNotProcess()
        {
            // arrange
            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var fileSystem = Substitute.For<IFileSystem>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);

            // act
            processor.ProcessList(new List<string>());

            // assert
            ruleVisitor.DidNotReceive().VisitRules(Arg.Any<string>(), Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(0, processor.FileCount);
        }

        [Test]
        public void ProcessList_ListOfPaths_ShouldOnlyProcessFilesInList()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\file1.SQL");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\db1\file2.sql");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\file3.sql");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\sproc\file4.Sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

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

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);
            globPatternMatcher.GetResultsInFullPath(TestHelper.GetTestFilePath(@"c:\dbscripts\db1\")).Returns(new[] { filePath2 });
            globPatternMatcher.GetResultsInFullPath(TestHelper.GetTestFilePath(@"c:\dbscripts\db2\sproc")).Returns(new[] { filePath4 });

            var f1 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\sproc");
            var f2 = TestHelper.GetTestFilePath(@"c:\dbscripts\db2\file3.sql");
            var multiPathString = $@" {f1}, {f2}";

            // act
            // tests quotes, extra spaces, commas, multiple items in the list
            processor.ProcessList(new List<string>
            {
                multiPathString,
                TestHelper.GetTestFilePath(@"c:\dbscripts\db1\")
            });

            // assert
            ruleVisitor.DidNotReceive().VisitRules(filePath1, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath3, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath4, Arg.Any<IEnumerable<IExtendedRuleException>>(), Arg.Any<Stream>());
            Assert.AreEqual(3, processor.FileCount);
        }

        [Test]
        public void ProcessList_InvalidPaths_ShouldProcessValidPaths()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\dbscripts\db1\file2.sql");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\dbscripts\db1\file3.sql");
            var invalidFilePath = TestHelper.GetTestFilePath(@"c:\invalid\invalid.sql");

            var ruleVisitor = Substitute.For<IRuleVisitor>();
            var reporter = Substitute.For<IReporter>();
            var pluginHandler = Substitute.For<IPluginHandler>();
            var globPatternMatcher = Substitute.For<IGlobPatternMatcher>();

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData("File1SQL")
                },
                {
                    filePath2, new MockFileData("File2SQL")
                }
            });

            var processor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, fileSystem, ruleList, globPatternMatcher);
            globPatternMatcher.GetResultsInFullPath(TestHelper.GetTestFilePath(@"c:\dbscripts\db1\")).Returns(new[] { filePath1, filePath2 });

            // act
            processor.ProcessList(new List<string> { invalidFilePath, TestHelper.GetTestFilePath(@"c:\dbscripts\db1\") });

            // assert
            ruleVisitor.DidNotReceive().VisitRules(invalidFilePath, Arg.Any<IEnumerable<IRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath1, Arg.Any<IEnumerable<IRuleException>>(), Arg.Any<Stream>());
            ruleVisitor.Received().VisitRules(filePath2, Arg.Any<IEnumerable<IRuleException>>(), Arg.Any<Stream>());
            reporter.Received().Report($@"{invalidFilePath} is not a valid file path.");
            Assert.AreEqual(2, processor.FileCount);
        }
    }
}
