using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class DisallowCursorsRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "disallow-cursors", "disallow-cursors-no-error",  typeof(DisallowCursorRule), new List<RuleViolation>()
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-one-error", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 3, 1)
                }
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-two-errors", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 3, 1),
                    new RuleViolation("disallow-cursors", 4, 1)
                }
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-one-error-mixed-state", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 4, 1)
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
