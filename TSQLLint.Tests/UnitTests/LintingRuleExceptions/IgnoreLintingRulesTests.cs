using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRuleExceptions
{
    public class IgnoreLintingRulesTests
    {
        private static readonly IEnumerable<RuleViolation> IntegrationTestTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"integration-test-two.sql"), "select-star", "text", 12, 8, RuleViolationSeverity.Error)
        };

        private static readonly IEnumerable<RuleViolation> EnableWithoutDisableRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"enable-without-disable.sql"), "select-star", "text", 7, 8, RuleViolationSeverity.Error)
        };

        private static readonly IEnumerable<RuleViolation> GlobalEnableWithoutDisableRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"global-enable-without-disable.sql"), "select-star", "text", 7, 8, RuleViolationSeverity.Error)
        };

        public static IEnumerable ExistingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(@"integration-test-two.sql", string.Empty, IntegrationTestTwoRuleViolations, 1).SetName("Ignore one select star rule enforce another");
                yield return new TestCaseData(@"global-disable.sql", string.Empty, new List<RuleViolation>(), 1).SetName("Globally disable rule varnings");
                yield return new TestCaseData(@"global-enable-without-disbling.sql", string.Empty, GlobalEnableWithoutDisableRuleViolations, 1).SetName("Globally enable without disabling first");
                yield return new TestCaseData(@"enable-without-disbling.sql", string.Empty, EnableWithoutDisableRuleViolations, 1).SetName("Globally enable without disabling first");
            }
        }

        private static string TestFileDirectory => Path.Combine(TestContext.CurrentContext.WorkDirectory, @"IntegrationTests/Configuration/TestFiles");

        [TestCaseSource(nameof(ExistingConfigTestCases))]
        public void RunExistingConfigTest(string testFile, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            testFile = $@"{TestContext.CurrentContext.TestDirectory}/UnitTests/LintingRuleExceptions/TestFiles/{testFile}";
            TestHelper.PerformApplicationTest(new List<string> { testFile }, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
