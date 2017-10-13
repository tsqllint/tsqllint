using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SelectStarRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "select-star", "select-star-no-error",  typeof(SelectStarRule), new List<RuleViolation>()
            },
            new object[]
            {
                "select-star", "select-star-one-error", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "select-star", startLine: 1, startColumn: 8)
                }
            },
            new object[]
            {
                "select-star", "select-star-two-errors", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "select-star", startLine: 1, startColumn: 8),
                    new RuleViolation(ruleName: "select-star", startLine: 3, startColumn: 14)
                }
            },
            new object[]
            {
                "select-star", "select-star-one-error-mixed-state", typeof(SelectStarRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "select-star", startLine: 3, startColumn: 12)
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
