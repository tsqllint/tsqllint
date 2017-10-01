using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.IntegrationTests
{
    public class IntegrationExistingConfigTests : IntegrationConfigBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var configFileGenerator = new ConfigFileGenerator(new TestReporter());
            configFileGenerator.WriteConfigFile(DefaultConfigFile);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            File.Delete(DefaultConfigFile);
        }

        private static readonly string InvalidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc-foo");

        private static readonly string ValidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc");
        private static readonly string TestFileTwo = Path.Combine(TestFileDirectory, @"TestFileSubDirectory\integration-test-two.sql");
        private static readonly string TestFileInvalidSyntax = Path.Combine(TestFileDirectory, @"invalid-syntax.sql");
        private static readonly List<RuleViolation> _AllRuleViolations = new List<RuleViolation>();
        private static readonly List<RuleViolation> _MultiFileRuleViolations = new List<RuleViolation>();

        private static string UsageString
        {
            get
            {
                return new CommandLineOptions(new string[0]).GetUsage();
            }
        }

        private static string TSqllVersion
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        private static readonly IEnumerable<RuleViolation> TestFileInvalidSyntaxRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(null, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error)
        };

        private static readonly IEnumerable<RuleViolation> TestFileTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(ruleName: "print-statement", startLine: 5, startColumn: 1),
        };

        public static IEnumerable<RuleViolation> MultiFileRuleViolations
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

        public static IEnumerable<RuleViolation> AllRuleViolations
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

        public static IEnumerable ExistingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new List<string> { "-c", ValidConfigFile },
                        UsageString,
                        new List<RuleViolation>(),
                        0)
                    .SetName("Config Args Valid No Lint Path");
                yield return new TestCaseData(
                        new List<string> { "-c", InvalidConfigFile },
                        string.Format("Existing config file not found at: {0} use the '--init' option to create if one does not exist or the '--force' option to overwrite", InvalidConfigFile),
                        new List<RuleViolation>(),
                        0)
                    .SetName("Config Args Invalid No Lint Path");
                yield return new TestCaseData(
                        new List<string> { "-c", InvalidConfigFile, TestFileOne },
                        string.Format("Existing config file not found at: {0} use the '--init' option to create if one does not exist or the '--force' option to overwrite", InvalidConfigFile),
                        new List<RuleViolation>(),
                        1)
                    .SetName("Config Args Invalid Lint Path");
                yield return new TestCaseData(
                        new List<string> { "-c", Path.Combine(TestFileDirectory, @".tsqllintrc"), TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("Config Args Valid Lint One File");
                yield return new TestCaseData(
                        new List<string> { TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("File Args Valid Lint One File");
                yield return new TestCaseData(
                        new List<string> { TestFileOne, TestFileTwo },
                        null,
                        MultiFileRuleViolations,
                        2)
                    .SetName("File Args Valid Lint Two Files");
                yield return new TestCaseData(
                        new List<string> { TestFileDirectory },
                        null,
                        AllRuleViolations,
                        3)
                    .SetName("File Args Valid Lint Directory");
                yield return new TestCaseData(
                        new List<string> { string.Empty },
                        UsageString,
                        new List<RuleViolation>(),
                        0)
                    .SetName("File Args Invalid No Files");
                yield return new TestCaseData(
                        new List<string> { @"invalid.sql" },
                        "invalid.sql is not a valid path.",
                        new List<RuleViolation>(),
                        0)
                    .SetName("File Args Invalid File Does Not Exist");
                yield return new TestCaseData(
                        new List<string> { @"c:\invalid\foo*.sql" },
                        @"Directory does not exit: c:\invalid",
                        new List<RuleViolation>(),
                        0)
                    .SetName("File Args Invalid due to Directory Does Not Exist");
                yield return new TestCaseData(
                        new List<string> { @"c:\invalid.sql" },
                        @"c:\invalid.sql is not a valid path.",
                        new List<RuleViolation>(),
                        0)
                    .SetName("File Args Invalid due to Path Does Not Exist");
                yield return new TestCaseData(
                        new List<string> { TestFileInvalidSyntax },
                        null,
                        TestFileInvalidSyntaxRuleViolations,
                        1)
                    .SetName("File Args Invalid due to Invalid Syntax");
                yield return new TestCaseData(
                        new List<string> { "-i", TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("Init Args Valid Existing Config File");
                yield return new TestCaseData(
                        new List<string> { "-f", TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("Force Args Valid");
                yield return new TestCaseData(
                        new List<string> { "-p" },
                        string.Format("Config file found at: {0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc"),
                        new List<RuleViolation>(),
                        0)
                    .SetName("Print Config Valid");
                yield return new TestCaseData(
                        new List<string> { "-v" },
                        string.Format("v{0}", TSqllVersion),
                        new List<RuleViolation>(),
                        0)
                    .SetName("Print Version Valid");
                yield return new TestCaseData(
                        new List<string>(),
                        UsageString,
                        new List<RuleViolation>(),
                        0)
                    .SetName("Invalid No Args");
            }
        }

        [TestCaseSource("ExistingConfigTestCases")]
        public void RunExistingConfigTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformApplicationTest(argumentsUnderTest, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}