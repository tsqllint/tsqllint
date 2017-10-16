using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.IntegrationTests
{
    public class ExistingConfigTests : IntegrationBaseTest
    {
        private static readonly string InvalidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc-foo");
        private static readonly string ValidConfigFile = Path.Combine(TestFileDirectory, @".tsqllintrc");
        private static readonly string TestFileTwo = Path.Combine(TestFileDirectory, @"TestFileSubDirectory\integration-test-two.sql");
        private static readonly string TestFileInvalidSyntax = Path.Combine(TestFileDirectory, @"invalid-syntax.sql");

        private static readonly string _ConfigNotFoundMessage = $"Config file not found at: {InvalidConfigFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite";
        private static readonly string _ConfigFoundMessage = $"Config file found at: {Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.tsqllintrc";

        private static List<RuleViolation> _AllRuleViolations;
        private static List<RuleViolation> _MultiFileRuleViolations;
            
        private static string UsageString => new CommandLineOptions(new string[0]).GetUsage();

        private static string TSqllVersion
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static IEnumerable<RuleViolation> MultiFileRuleViolations
        {
            get
            {
                if (_MultiFileRuleViolations != null)
                {
                    return _MultiFileRuleViolations;
                }
                
                _MultiFileRuleViolations = new List<RuleViolation>();
                _MultiFileRuleViolations.AddRange(TestFileOneRuleViolations);
                _MultiFileRuleViolations.AddRange(TestFileTwoRuleViolations);

                return _MultiFileRuleViolations;
            }
        }

        public static IEnumerable<RuleViolation> AllRuleViolations
        {
            get
            {
                // change if used more than once
                _AllRuleViolations = new List<RuleViolation>();
                _AllRuleViolations.AddRange(TestFileOneRuleViolations);
                _AllRuleViolations.AddRange(TestFileTwoRuleViolations);
                _AllRuleViolations.AddRange(TestFileInvalidSyntaxRuleViolations);

                return _AllRuleViolations;
            }
        }

        public static IEnumerable ExistingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> { "-c", ValidConfigFile }, UsageString, new List<RuleViolation>(), 0).SetName("Config Args Valid No Lint Path");
                yield return new TestCaseData(new List<string> { "-c", InvalidConfigFile }, _ConfigNotFoundMessage, new List<RuleViolation>(), 0).SetName("Config Args Invalid No Lint Path");
                yield return new TestCaseData(new List<string> { "-c", InvalidConfigFile, TestFileOne }, _ConfigNotFoundMessage, new List<RuleViolation>(), 1).SetName("Config Args Invalid Lint Path");
                yield return new TestCaseData(new List<string> { "-c", Path.Combine(TestFileDirectory, @".tsqllintrc"), TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Config Args Valid Lint One File");
                yield return new TestCaseData(new List<string> { TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("File Args Valid Lint One File");
                yield return new TestCaseData(new List<string> { TestFileOne, TestFileTwo }, null, MultiFileRuleViolations, 2).SetName("File Args Valid Lint Two Files");
                yield return new TestCaseData(new List<string> { TestFileTwo, TestFileOne }, null, MultiFileRuleViolations, 2).SetName("File Args Valid Lint Two Files, Changed Order");
                yield return new TestCaseData(new List<string> { TestFileDirectory }, null, AllRuleViolations, 3).SetName("File Args Valid Lint Directory");
                yield return new TestCaseData(new List<string> { string.Empty }, UsageString, new List<RuleViolation>(), 0).SetName("File Args Invalid No Files");
                yield return new TestCaseData(new List<string> { @"invalid.sql" }, "invalid.sql is not a valid path.", new List<RuleViolation>(), 0).SetName("File Args Invalid File Does Not Exist");
                yield return new TestCaseData(new List<string> { @"c:\invalid\foo*.sql" }, @"Directory does not exit: c:\invalid", new List<RuleViolation>(), 0).SetName("File Args Invalid due to Directory Does Not Exist");
                yield return new TestCaseData(new List<string> { @"c:\invalid.sql" }, @"c:\invalid.sql is not a valid path.", new List<RuleViolation>(), 0).SetName("File Args Invalid due to Path Does Not Exist");
                yield return new TestCaseData(new List<string> { TestFileInvalidSyntax }, null, TestFileInvalidSyntaxRuleViolations, 1).SetName("File Args Invalid due to Invalid Syntax");
                yield return new TestCaseData(new List<string> { "-i", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Init Args Valid Existing Config File");
                yield return new TestCaseData(new List<string> { "-f", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Force Args Valid");
                yield return new TestCaseData(new List<string> { "-p" }, _ConfigFoundMessage, new List<RuleViolation>(), 0).SetName("Print Config Valid");
                yield return new TestCaseData(new List<string> { "-v" }, $"v{TSqllVersion}", new List<RuleViolation>(), 0).SetName("Print Version Valid");
                yield return new TestCaseData(new List<string>(), UsageString, new List<RuleViolation>(), 0).SetName("Invalid No Args");
                yield return new TestCaseData(new List<string> { "-i", TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("Init Args Valid Missing Config File");
                yield return new TestCaseData(new List<string> { TestFileOne }, null, TestFileOneRuleViolations, 1).SetName("File Args Valid Lint One File");
            }
        }

        [TestCaseSource(nameof(ExistingConfigTestCases))]
        public void RunExistingConfigTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformApplicationTest(argumentsUnderTest, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
