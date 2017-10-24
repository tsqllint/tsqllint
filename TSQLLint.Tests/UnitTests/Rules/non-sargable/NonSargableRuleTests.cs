using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class NonSargableRuleTests
    {
        private static readonly object[] testCases =
        {
            new object[]
            {
                "non-sargable", "non-sargable-no-error",  typeof(NonSargableRule), new List<RuleViolation>()
            },
            new object[]
            {
                "non-sargable", "non-sargable-one-error",  typeof(NonSargableRule), new List<RuleViolation>()
                {
                    new RuleViolation(ruleName: "non-sargable", startLine: 1, startColumn: 25)
                }
            },
            new object[]
            {
                "non-sargable", "non-sargable-multi-error",  typeof(NonSargableRule), new List<RuleViolation>()
                {
                    new RuleViolation(ruleName: "non-sargable", startLine: 1, startColumn: 37),
                    new RuleViolation(ruleName: "non-sargable", startLine: 1, startColumn: 55),
                    new RuleViolation(ruleName: "non-sargable", startLine: 3, startColumn: 37),
                    new RuleViolation(ruleName: "non-sargable", startLine: 5, startColumn: 31),
                    new RuleViolation(ruleName: "non-sargable", startLine: 7, startColumn: 25),
                    new RuleViolation(ruleName: "non-sargable", startLine: 9, startColumn: 28),
                    new RuleViolation(ruleName: "non-sargable", startLine: 11, startColumn: 25),
                    new RuleViolation(ruleName: "non-sargable", startLine: 13, startColumn: 25),
                    new RuleViolation(ruleName: "non-sargable", startLine: 15, startColumn: 25),
                    new RuleViolation(ruleName: "non-sargable", startLine: 17, startColumn: 25),
                    new RuleViolation(ruleName: "non-sargable", startLine: 19, startColumn: 28),
                    new RuleViolation(ruleName: "non-sargable", startLine: 21, startColumn: 28),
                    new RuleViolation(ruleName: "non-sargable", startLine: 29, startColumn: 12),
                    new RuleViolation(ruleName: "non-sargable", startLine: 37, startColumn: 11),
                    new RuleViolation(ruleName: "non-sargable", startLine: 45, startColumn: 14),
                    new RuleViolation(ruleName: "non-sargable", startLine: 54, startColumn: 13)
                }
            }
        };

        [Test, TestCaseSource("testCases")]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
