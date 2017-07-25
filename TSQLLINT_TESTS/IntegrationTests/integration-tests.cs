using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_CONSOLE;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;
using TSQLLINT_LIB_TESTS.Helpers;

namespace TSQLLINT_LIB_TESTS.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly string DefaultConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc");

        [OneTimeSetUp]
        public void Setup()
        {
            var configFileGenerator = new ConfigFileGenerator(new TestReporter());
            configFileGenerator.WriteConfigFile(DefaultConfigFile);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            File.Delete(DefaultConfigFile);
        }

        #region Test Values

        private readonly RuleViolationCompare comparer = new RuleViolationCompare();
        private static readonly string TestFileDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\IntegrationTests\TestFiles");

        private static readonly string ValidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc");
        private static readonly string TestFileOne = Path.Combine(TestFileDirectory, @"integration-test-one.sql");
        private static readonly string TestFileTwo = Path.Combine(TestFileDirectory, @"TestFileSubDirectory\integration-test-two.sql");
        private static readonly string TestFileInvalidSyntax = Path.Combine(TestFileDirectory, @"invalid-syntax.sql");

        private static string _GetUsageString;
        private static string GetUsageString
        {
            get
            {
                if (string.IsNullOrEmpty(_GetUsageString))
                {
                    var consoleCommandLineOptionParser = new CommandLineOptions(new string[0]);
                    _GetUsageString = consoleCommandLineOptionParser.GetUsage();
                }
                return _GetUsageString;
            }
        }

        private static string _TSqllVersion;
        private static string TSqllVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_TSqllVersion))
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    _TSqllVersion = fvi.FileVersion;
                }
                return _TSqllVersion;
            }
        }

        private static readonly List<RuleViolation> TestFileInvalidSyntaxRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(null, "TSQL not syntactically correct")
        };

        private static readonly List<RuleViolation> TestFileOneRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(ruleName: "conditional-begin-end", startLine: 2, startColumn: 1),
            new RuleViolation(ruleName: "data-compression", startLine: 6, startColumn: 1),
            new RuleViolation(ruleName: "data-type-length", startLine: 13, startColumn: 16),
            new RuleViolation(ruleName: "disallow-cursors", startLine: 17, startColumn: 1),
            new RuleViolation(ruleName: "information-schema", startLine: 20, startColumn: 27),
            new RuleViolation(ruleName: "keyword-capitalization", startLine: 23, startColumn: 1),
            new RuleViolation(ruleName: "multi-table-alias", startLine: 27, startColumn: 10),
            new RuleViolation(ruleName: "object-property", startLine: 38, startColumn: 7),
            new RuleViolation(ruleName: "print-statement", startLine: 42, startColumn: 1),
            new RuleViolation(ruleName: "schema-qualify", startLine: 45, startColumn: 17),
            new RuleViolation(ruleName: "select-star", startLine: 48, startColumn: 8),
            new RuleViolation(ruleName: "semicolon-termination", startLine: 51, startColumn: 31),
            new RuleViolation(ruleName: "set-ansi", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-nocount", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-quoted-identifier", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-transaction-isolation-level", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "upper-lower", startLine: 59, startColumn: 8),
        };

        private static readonly List<RuleViolation> TestFileTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(ruleName: "print-statement", startLine: 5, startColumn: 1),
        };

        private static readonly List<RuleViolation> _MultiFileRuleViolations = new List<RuleViolation>();
        public static List<RuleViolation> MultiFileRuleViolations
        {
            get
            {
                if (_MultiFileRuleViolations.Count == 0)
                {
                    _MultiFileRuleViolations.AddRange(TestFileOneRuleViolations);
                    _MultiFileRuleViolations.AddRange(TestFileTwoRuleViolations);
                }

                return _MultiFileRuleViolations;
            }
        }

        private static readonly List<RuleViolation> _AllRuleViolations = new List<RuleViolation>();
        public static List<RuleViolation> AllRuleViolations
        {
            get
            {
                if (_AllRuleViolations.Count == 0)
                {
                    _AllRuleViolations.AddRange(TestFileOneRuleViolations);
                    _AllRuleViolations.AddRange(TestFileTwoRuleViolations);
                    _AllRuleViolations.AddRange(TestFileInvalidSyntaxRuleViolations);
                }

                return _AllRuleViolations;
            }
        }

        #endregion

        #region Config Argument Test Cases

        public static readonly object[] ConfigArgs_Valid_NoLintPath = {
          new object[]
          {
            new List<string> { "-c" , ValidConfigFile },
            GetUsageString,
            new List<RuleViolation>(),
            0
          }, 
        };

        public static readonly object[] ConfigArgs_InValid_NoLintPath = {
          new object[]
          {
            new List<string> { "-c" , Path.Combine(TestFileDirectory, @".tsqllintrc-foo") },
            GetUsageString,
            new List<RuleViolation>(),
            0
          }, 
        };

        public static readonly object[] ConfigArgs_Valid_LintOneFile = {
          new object[]
          {
            new List<string> { "-c" , Path.Combine(TestFileDirectory, @".tsqllintrc"), "-f", TestFileOne },
            null,
            TestFileOneRuleViolations,
            1
          }, 
        };

        #endregion

        #region File Argument Test Cases

        public static readonly object[] FileArgs_Valid_LintOneFile = {
          new object[]
          {
            new List<string> { "-f", TestFileOne },
            null,
            TestFileOneRuleViolations,
            1
          }, 
        };

        public static readonly object[] FileArgs_Valid_LintTwoFiles = {
          new object[]
          {
            new List<string> { "-f", string.Format("{0}, {1}", TestFileOne, TestFileTwo) },
            null,
            MultiFileRuleViolations,
            2
          }, 
        };

        public static readonly object[] FileArgs_Valid_LintDirectory = {
          new object[]
          {
            new List<string> { "-f", TestFileDirectory },
            null,
            AllRuleViolations,
            3
          } 
        };

        public static readonly object[] FileArgs_InValid_NoFile = {
          new object[]
          {
            new List<string> { "-f", "" },
            GetUsageString,
            new List<RuleViolation>(),
            0
          }
        };

        public static readonly object[] FileArgs_InValid_FileNotExists = {
          new object[]
          {
            new List<string> { "-f", "foo.sql" },
            "\nfoo.sql is not a valid path.",
            new List<RuleViolation>(),
            0
          }, 
        };

        public static readonly object[] FileArgs_InValid_InvalidSyntax = {
          new object[]
          {
            new List<string> { "-f", TestFileInvalidSyntax },
            null,
            TestFileInvalidSyntaxRuleViolations,
            1
          }, 
        };

        #endregion

        #region Init Argument Test Cases

        public static readonly object[] InitArgs_Valid = {
          new object[]
          {
            new List<string> { "-i" },
            string.Format("Created default config file {0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc"),
            new List<RuleViolation>(),
            0
          }, 
        };

        #endregion

        #region Print Config Test Cases

        public static readonly object[] Print_Config_Valid = {
          new object[]
          {
            new List<string> { "-p" },
            string.Format("Default config file found at: {0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc"),
            new List<RuleViolation>(),
            0
          }, 
        };

        #endregion

        #region Print Version Test Cases

        public static readonly object[] Print_Version_Valid = {
          new object[]
          {
            new List<string> { "-v" },
            string.Format("v{0}", TSqllVersion),
            new List<RuleViolation>(),
            0
          }, 
        };

        #endregion

        [Test,
            TestCaseSource("ConfigArgs_Valid_NoLintPath"),
            TestCaseSource("ConfigArgs_InValid_NoLintPath"),
            TestCaseSource("ConfigArgs_Valid_LintOneFile"),
            TestCaseSource("InitArgs_Valid"),
            TestCaseSource("FileArgs_Valid_LintOneFile"),
            TestCaseSource("FileArgs_Valid_LintTwoFiles"),
            TestCaseSource("FileArgs_Valid_LintDirectory"),
            TestCaseSource("FileArgs_InValid_NoFile"),
            TestCaseSource("FileArgs_InValid_FileNotExists"),
            TestCaseSource("FileArgs_InValid_InvalidSyntax"),
            TestCaseSource("Print_Config_Valid"),
            TestCaseSource("Print_Version_Valid")
        ]
        public void RunIntegrationTestUseCase(List<string> args, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            // arrange
            var appArgs = args.ToArray();
            var testReporter = new TestReporter();
            var application = new Application(appArgs, testReporter);

            // act
            application.Run();

            // assert
            Assert.AreEqual(expectedMessage, testReporter.Message);

            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();
            var reportedRuleViolations = testReporter.RuleViolations.OrderBy(o => o.Line).ToList();
            Assert.AreEqual(expectedRuleViolations.Count, expectedRuleViolations.Count);
            CollectionAssert.AreEqual(expectedRuleViolations, reportedRuleViolations, comparer);

            Assert.AreEqual(expectedFileCount, testReporter.FileCount);
        }

        private class TestReporter : IReporter
        {
            public string Message ;
            public List<RuleViolation> RuleViolations = new List<RuleViolation>();
            public int FileCount;

            public void ReportResults(List<RuleViolation> ruleViolations, TimeSpan timespan, int fileCount)
            {
                RuleViolations = ruleViolations;
                FileCount = fileCount;
            }

            public void Report(string message)
            {
                Message = message;
            }
        }
    }
}
