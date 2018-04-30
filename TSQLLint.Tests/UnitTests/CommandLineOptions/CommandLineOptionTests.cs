using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules.RuleViolations;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.UnitTests.CommandLineOptions
{
    [TestFixture]
    public class CommandLineOptionTests
    {
        private static readonly string InvalidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc-foo");
        private static readonly string ValidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc");
        private static readonly string TestFileTwo = Path.Combine(TestFileDirectory, @"TestFileSubDirectory/integration-test-two.sql");
        private static readonly string TestFileInvalidSyntax = Path.Combine(TestFileDirectory, @"invalid-syntax.sql");
        private static readonly string TestFileInvalidEncoding = Path.Combine(TestFileDirectory, @"invalid-encoding.sql");
        private static readonly string ConfigNotFoundMessage = $"Config file not found at: {InvalidConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite";
        private static readonly string ConfigFoundMessage = $"Config file found at: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc")}";
        private static readonly string NoPluginsFound = "Did not find any plugins";

        private static readonly IEnumerable<RuleViolation> TestFileOneRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("conditional-begin-end", 2, 1),
            new RuleViolation("data-compression", 6, 1),
            new RuleViolation("data-type-length", 13, 16),
            new RuleViolation("disallow-cursors", 17, 1),
            new RuleViolation("information-schema", 20, 27),
            new RuleViolation("non-sargable", 38, 7),
            new RuleViolation("keyword-capitalization", 23, 1),
            new RuleViolation("multi-table-alias", 27, 10),
            new RuleViolation("object-property", 38, 7),
            new RuleViolation("print-statement", 42, 1),
            new RuleViolation("schema-qualify", 45, 17),
            new RuleViolation("select-star", 48, 8),
            new RuleViolation("semicolon-termination", 51, 31),
            new RuleViolation("set-ansi", 1, 1),
            new RuleViolation("set-nocount", 1, 1),
            new RuleViolation("set-quoted-identifier", 1, 1),
            new RuleViolation("set-transaction-isolation-level", 1, 1),
            new RuleViolation("upper-lower", 59, 1),
            new RuleViolation("non-sargable", 59, 41)
        };

        private static readonly IEnumerable<RuleViolation> TestFileInvalidSyntaxRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("keyword-capitalization", 1, 1),
            new RuleViolation("set-ansi", 1, 1),
            new RuleViolation("set-nocount", 1, 1),
            new RuleViolation("set-quoted-identifier", 1, 1),
            new RuleViolation("set-transaction-isolation-level", 1, 1),
            new RuleViolation("invalid-syntax", 1, 1),
        };

        private static readonly IEnumerable<RuleViolation> TestFileInvalidEncodingRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("invalid-syntax", 6, 23),
        };

        private static readonly IEnumerable<RuleViolation> TestFileTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("print-statement", 5, 1)
        };

        private static List<RuleViolation> allRuleViolations;
        private static List<RuleViolation> multiFileRuleViolations;

        private static string TestFileBase => TestContext.CurrentContext.WorkDirectory;

        private static string TestFileDirectory => Path.Combine(TestFileBase, @"UnitTests/CommandLineOptions/TestFiles");

        private static string TestFileOne => Path.Combine(TestFileDirectory, @"integration-test-one.sql");

        private static string UsageString => new Infrastructure.CommandLineOptions.CommandLineOptions(new string[] { }).GetUsage();

        private static string TSqllVersion
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        private static IEnumerable<RuleViolation> MultiFileRuleViolations
        {
            get
            {
                if (multiFileRuleViolations != null)
                {
                    return multiFileRuleViolations;
                }

                multiFileRuleViolations = new List<RuleViolation>();
                multiFileRuleViolations.AddRange(TestFileOneRuleViolations);
                multiFileRuleViolations.AddRange(TestFileTwoRuleViolations);

                return multiFileRuleViolations;
            }
        }

        private static IEnumerable<RuleViolation> AllRuleViolations
        {
            get
            {
                // change if used more than once
                allRuleViolations = new List<RuleViolation>();
                allRuleViolations.AddRange(TestFileOneRuleViolations);
                allRuleViolations.AddRange(TestFileTwoRuleViolations);
                allRuleViolations.AddRange(TestFileInvalidSyntaxRuleViolations);
                allRuleViolations.AddRange(TestFileInvalidEncodingRuleViolations);

                return allRuleViolations;
            }
        }

        private static IEnumerable CommandLineOptionTestCases
        {
            get
            {
                // no args
                yield return new TestCaseData(new List<string>(), UsageString, new List<RuleViolation>(), 0).SetName("Invalid No Args");
                yield return new TestCaseData(new List<string> { string.Empty }, UsageString, new List<RuleViolation>(), 0).SetName("File Args Invalid No Files");

                // args and linting targets
                yield return new TestCaseData(new List<string> { "-c", ValidConfigFile }, UsageString, new List<RuleViolation>(), 0).SetName("Config Args Valid No Lint Path");
                yield return new TestCaseData(new List<string> { "-c", InvalidConfigFile }, ConfigNotFoundMessage, new List<RuleViolation>(), 0).SetName("Config Args Invalid No Lint Path");
                yield return new TestCaseData(new List<string> { "-c", InvalidConfigFile, TestFileOne }, ConfigNotFoundMessage, new List<RuleViolation>(), 1).SetName("Config Args Invalid Lint Path");
                yield return new TestCaseData(new List<string> { "-c", Path.Combine(TestFileDirectory, @".tsqllintrc"), TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Config Args Valid Lint One File");
                yield return new TestCaseData(new List<string> { "-p" }, ConfigFoundMessage, new List<RuleViolation>(), 0).SetName("Print Config Valid");
                yield return new TestCaseData(new List<string> { "-l" }, NoPluginsFound, new List<RuleViolation>(), 0).SetName("List Plugins Valid");
                yield return new TestCaseData(new List<string> { "-v" }, $"v{TSqllVersion}", new List<RuleViolation>(), 0).SetName("Print Version Valid");
                yield return new TestCaseData(new List<string> { "-i", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Init Args Valid Missing Config File");
                yield return new TestCaseData(new List<string> { "-i", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Init Args Valid Existing Config File");
                yield return new TestCaseData(new List<string> { "-f", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Force Args Valid");

                // invalid linting targets
                yield return new TestCaseData(new List<string> { @"invalid.sql" }, "invalid.sql is not a valid file path.", new List<RuleViolation>(), 0).SetName("File Args Invalid File Does Not Exist");
                yield return new TestCaseData(new List<string> { @"c:/invalid/foo*.sql" }, $"Directory does not exist: {Path.Combine($"c:{Path.DirectorySeparatorChar}", "invalid")}", new List<RuleViolation>(), 0).SetName("File Args Invalid Directory Does Not Exist");
                yield return new TestCaseData(new List<string> { @"c:/invalid.sql" }, @"c:/invalid.sql is not a valid file path.", new List<RuleViolation>(), 0).SetName("File Args Invalid due to Path Does Not Exist");

                // valid linting files and directories
                yield return new TestCaseData(new List<string> { TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("File Args Valid Lint One File");
                yield return new TestCaseData(new List<string> { TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("File Args Valid Lint One File");
                yield return new TestCaseData(new List<string> { TestFileOne, TestFileTwo }, null, MultiFileRuleViolations, 2).SetName("File Args Valid Lint Two Files");
                yield return new TestCaseData(new List<string> { TestFileTwo, TestFileOne }, null, MultiFileRuleViolations, 2).SetName("File Args Valid Lint Two Files, Changed Order");
                yield return new TestCaseData(new List<string> { TestFileDirectory }, null, AllRuleViolations, 3).SetName("File Args Valid Lint Directory");
                yield return new TestCaseData(new List<string> { TestFileInvalidSyntax }, null, TestFileInvalidSyntaxRuleViolations, 1).SetName("Invalid sql script");
                yield return new TestCaseData(new List<string> { TestFileInvalidEncoding }, null, TestFileInvalidEncodingRuleViolations, 1).SetName("File encoding not valid");
            }
        }

        [TestCaseSource(nameof(CommandLineOptionTestCases))]
        public void RunExistingConfigTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            TestHelper.PerformApplicationTest(argumentsUnderTest, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
