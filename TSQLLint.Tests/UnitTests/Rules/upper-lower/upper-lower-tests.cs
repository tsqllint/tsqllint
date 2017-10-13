using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class UpperLowerRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "upper-lower", "upper-lower-no-error",  typeof(UpperLowerRule), new List<RuleViolation>()
            },
            new object[]
            {
                "upper-lower", "upper-lower-one-error", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "upper-lower", startLine: 1, startColumn: 8)
                }
            },
            new object[]
            {
                "upper-lower", "upper-lower-two-errors", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "upper-lower", startLine: 1, startColumn: 8),
                    new RuleViolation(ruleName: "upper-lower", startLine: 2, startColumn: 8)
                }
            },
            new object[]
            {
                "upper-lower", "upper-lower-one-error-mixed-state", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "upper-lower", startLine: 3, startColumn: 8)
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
