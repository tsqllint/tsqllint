using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class UpperLowerRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "upper-lower", "upper-lower-no-error",  typeof(UpperLowerRule), new List<RuleViolation>()
            },
            new object[]
            {
                "upper-lower", "upper-lower-one-error", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation("upper-lower", 1, 1)
                }
            },
            new object[]
            {
                "upper-lower", "upper-lower-one-error-mixed-state", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation("upper-lower", 3, 1)
                }
            }
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
