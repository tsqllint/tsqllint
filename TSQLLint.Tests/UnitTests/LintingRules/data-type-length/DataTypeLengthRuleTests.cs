using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DataTypeLengthRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "data-type-length", "data-type-length-no-error",  typeof(DataTypeLengthRule), new List<RuleViolation>()
            },
            new object[]
            {
                "data-type-length", "data-type-length-one-error", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation("data-type-length", 3, 19)
                }
            },

            new object[]
            {
                "data-type-length", "data-type-length-one-error-mixed-state", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation("data-type-length", 7, 15)
                }
            },
            new object[]
            {
                "data-type-length", "data-type-length-all-errors", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation("data-type-length", 2,  17),
                    new RuleViolation("data-type-length", 3,  20),
                    new RuleViolation("data-type-length", 4,  23),
                    new RuleViolation("data-type-length", 5,  19),
                    new RuleViolation("data-type-length", 6,  20),
                    new RuleViolation("data-type-length", 7,  22),
                    new RuleViolation("data-type-length", 8,  22),
                    new RuleViolation("data-type-length", 9,  22),
                    new RuleViolation("data-type-length", 10, 19)
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
