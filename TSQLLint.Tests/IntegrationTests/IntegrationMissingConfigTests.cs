using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.IntegrationTests
{
    public class IntegrationMissingConfigTests : IntegrationConfigBase
    {
        public static IEnumerable MissingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<string>
                    {
                        "-i", TestFileOne
                    },
                    null,
                    TestFileOneRuleViolations,
                    1).SetName("Init Args Valid Missing Config File");
                yield return new TestCaseData(
                    new List<string> { TestFileOne },
                    null,
                    TestFileOneRuleViolations,
                    1).SetName("File Args Valid Lint One File");
            }
        }

        [TestCaseSource("MissingConfigTestCases")]
        public void RunMissingConfigTest(List<string> args, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformApplicationTest(args, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}