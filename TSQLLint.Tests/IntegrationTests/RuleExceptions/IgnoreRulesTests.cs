using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Rules.RuleViolations;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.IntegrationTests.RuleExceptions
{
    public class IgnoreRulesTests : IntegrationBaseTest
    {
        protected static readonly IEnumerable<RuleViolation> IntegrationTestTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"integration-test-two.sql"), "select-star", "text", 12, 8, RuleViolationSeverity.Error)
        };

        protected static readonly IEnumerable<RuleViolation> EnableWithoutDisableRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"enable-without-disable.sql"), "select-star", "text", 7, 8, RuleViolationSeverity.Error)
        };

        protected static readonly IEnumerable<RuleViolation> GlobalEnableWithoutDisableRuleViolations = new List<RuleViolation>
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

        [TestCaseSource(nameof(ExistingConfigTestCases))]
        public void RunExistingConfigTest(string testFile, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            testFile = $@"{TestContext.CurrentContext.TestDirectory}/IntegrationTests/RuleExceptions/TestFiles/{testFile}";
            PerformApplicationTest(new List<string> { testFile }, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
