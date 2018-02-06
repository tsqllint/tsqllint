using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.IntegrationTests.Configuration
{
    [TestFixture]
    public class CommandLineOptionTests : IntegrationBaseTest
    {
        private static readonly string InvalidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc-foo");
        private static readonly string ValidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc");
        private static readonly string TestFileTwo = Path.Combine(TestFileDirectory, @"TestFileSubDirectory/integration-test-two.sql");
        private static readonly string TestFileInvalidSyntax = Path.Combine(TestFileDirectory, @"invalid-syntax.sql");
        private static readonly string TestFileInvalidEncoding = Path.Combine(TestFileDirectory, @"invalid-encoding.sql");
        private static readonly string ConfigNotFoundMessage = $"Config file not found at: {InvalidConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite";
        private static readonly string ConfigFoundMessage = $"Config file found at: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc")}";
        private static readonly string NoPluginsFound = "Did not find any plugins";

        private static List<RuleViolation> allRuleViolations;
        private static List<RuleViolation> multiFileRuleViolations;

        public static IEnumerable<RuleViolation> MultiFileRuleViolations
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

        public static IEnumerable<RuleViolation> AllRuleViolations
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

        public static IEnumerable CommandLineOptionTestCases
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
            PerformApplicationTest(argumentsUnderTest, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
