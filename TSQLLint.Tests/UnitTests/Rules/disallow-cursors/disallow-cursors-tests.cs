using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class DisallowCursorsRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "disallow-cursors", "disallow-cursors-no-error",  typeof(DisallowCursorRule), new List<RuleViolation>()
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-one-error", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "disallow-cursors", startLine: 3, startColumn: 1)
                }
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-two-errors", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "disallow-cursors", startLine: 3, startColumn: 1),
                    new RuleViolation(ruleName: "disallow-cursors", startLine: 4, startColumn: 1)
                }
            },
            new object[]
            {
                "disallow-cursors", "disallow-cursors-one-error-mixed-state", typeof(DisallowCursorRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "disallow-cursors", startLine: 4, startColumn: 1)
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
