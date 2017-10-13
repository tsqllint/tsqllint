using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class DataTypeLengthRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "data-type-length", "data-type-length-no-error",  typeof(DataTypeLengthRule), new List<RuleViolation>()
            },
            new object[]
            {
                "data-type-length", "data-type-length-one-error", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "data-type-length", startLine: 3, startColumn: 19)
                }
            },

            new object[]
            {
                "data-type-length", "data-type-length-one-error-mixed-state", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "data-type-length", startLine: 7, startColumn: 15)
                }
            },
            new object[]
            {
                "data-type-length", "data-type-length-all-errors", typeof(DataTypeLengthRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "data-type-length", startLine: 2,  startColumn: 17),
                    new RuleViolation(ruleName: "data-type-length", startLine: 3,  startColumn: 20),
                    new RuleViolation(ruleName: "data-type-length", startLine: 4,  startColumn: 23),
                    new RuleViolation(ruleName: "data-type-length", startLine: 5,  startColumn: 19),
                    new RuleViolation(ruleName: "data-type-length", startLine: 6,  startColumn: 20),
                    new RuleViolation(ruleName: "data-type-length", startLine: 7,  startColumn: 22),
                    new RuleViolation(ruleName: "data-type-length", startLine: 8,  startColumn: 22),
                    new RuleViolation(ruleName: "data-type-length", startLine: 9,  startColumn: 22),
                    new RuleViolation(ruleName: "data-type-length", startLine: 10, startColumn: 19),
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
