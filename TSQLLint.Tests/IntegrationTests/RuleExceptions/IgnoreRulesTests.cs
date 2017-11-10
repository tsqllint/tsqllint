using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.IntegrationTests
{
    public class IgnoreRulesTests : IntegrationBaseTest
    {
        protected static readonly string TestFileDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\IntegrationTests\RuleExceptions\TestFiles");

        protected static readonly IEnumerable<RuleViolation> IntegrationTestTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(Path.Combine(TestFileDirectory, @"integration-test-two.sql"), "select-star", "text", 12, 8, RuleViolationSeverity.Error)
        };

        public static IEnumerable ExistingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> { Path.Combine(TestFileDirectory, @"integration-test-two.sql") }, string.Empty, IntegrationTestTwoRuleViolations, 1).SetName("Ignore one select star rule enforce another");
                yield return new TestCaseData(new List<string> { Path.Combine(TestFileDirectory, @"global-disable.sql") }, string.Empty, new List<RuleViolation>(), 1).SetName("Globally disable rule varnings");
            }
        }

        [TestCaseSource(nameof(ExistingConfigTestCases))]
        public void RunExistingConfigTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformApplicationTest(argumentsUnderTest, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}
