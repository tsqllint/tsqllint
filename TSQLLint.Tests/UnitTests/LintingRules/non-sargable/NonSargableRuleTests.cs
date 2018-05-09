using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class NonSargableRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "non-sargable", "non-sargable-no-error",  typeof(NonSargableRule), new List<RuleViolation>()
            },
            new object[]
            {
                "non-sargable", "non-sargable-one-error-where-clause",  typeof(NonSargableRule), new List<RuleViolation>
                {
                    new RuleViolation("non-sargable", 1, 25)
                }
            },
            new object[]
            {
                "non-sargable", "non-sargable-one-error-join-table",  typeof(NonSargableRule), new List<RuleViolation>
                {
                    new RuleViolation("non-sargable", 2, 19)
                }
            },
            new object[]
            {
                "non-sargable", "non-sargable-isnull-one-clause-one-error",  typeof(NonSargableRule), new List<RuleViolation>
                {
                    new RuleViolation("non-sargable", 2, 7)
                }
            },
            new object[]
            {
                "non-sargable", "non-sargable-isnull-multi-statement-one-error",  typeof(NonSargableRule), new List<RuleViolation>
                {
                    new RuleViolation("non-sargable", 6, 7)
                }
            },
            new object[]
            {
                "non-sargable", "non-sargable-multi-error",  typeof(NonSargableRule), new List<RuleViolation>
                {
                    new RuleViolation("non-sargable", 1, 37),
                    new RuleViolation("non-sargable", 1, 55),
                    new RuleViolation("non-sargable", 3, 37),
                    new RuleViolation("non-sargable", 5, 31),
                    new RuleViolation("non-sargable", 7, 25),
                    new RuleViolation("non-sargable", 9, 28),
                    new RuleViolation("non-sargable", 11, 25),
                    new RuleViolation("non-sargable", 13, 25),
                    new RuleViolation("non-sargable", 15, 25),
                    new RuleViolation("non-sargable", 17, 25),
                    new RuleViolation("non-sargable", 19, 28),
                    new RuleViolation("non-sargable", 21, 28),
                    new RuleViolation("non-sargable", 29, 12),
                    new RuleViolation("non-sargable", 37, 11),
                    new RuleViolation("non-sargable", 45, 13),
                    new RuleViolation("non-sargable", 54, 13)
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
