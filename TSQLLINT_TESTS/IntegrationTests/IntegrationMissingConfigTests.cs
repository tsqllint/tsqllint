using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.IntegrationTests
{
    public class IntegrationMissingConfigTests : IntegrationConfigBase
    {
        public static IEnumerable MissingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new List<string> { "-i", TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("Init Args Valid Missing Config File");
            }
        }

        [TestCaseSource("MissingConfigTestCases")]
        public void RunMissingConfigTest(List<string> args, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformTest(args, expectedMessage, expectedRuleViolations, expectedFileCount);
        }
    }
}