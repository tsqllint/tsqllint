using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SelectStarRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "select-star", "select-star-no-error",  typeof(SelectStarRule), new List<RuleViolation>()
            },
            new object[]
            {
                "select-star", "select-star-one-error", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation("select-star", 1, 8)
                }
            },
            new object[]
            {
                "select-star", "select-star-two-errors", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation("select-star", 1, 8),
                    new RuleViolation("select-star", 3, 14)
                }
            },
            new object[]
            {
                "select-star", "select-star-one-error-mixed-state", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation("select-star", 3, 12)
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
