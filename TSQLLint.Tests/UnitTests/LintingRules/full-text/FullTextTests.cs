using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    [TestFixture]
    public class FullTextTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "full-text", "full-text-no-error", typeof(FullTextRule), new List<RuleViolation>()
            },
            new object[]
            {
                "full-text", "full-text-one-error-mixed-state", typeof(FullTextRule), new List<RuleViolation>
                {
                    new RuleViolation("full-text", 6, 8)
                }
            },
            new object[]
            {
                "full-text", "full-text-one-error", typeof(FullTextRule), new List<RuleViolation>
                {
                    new RuleViolation("full-text", 4, 8),
                }
            },
            new object[]
            {
                "full-text", "full-text-two-errors", typeof(FullTextRule), new List<RuleViolation>
                {
                    new RuleViolation("full-text", 4, 8),
                    new RuleViolation("full-text", 11, 7)
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
