using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SetVariableRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "set-variable", "set-variable-no-error",  typeof(SetVariableRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-variable", "set-variable-one-error-mixed-state", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "set-variable", startLine: 6, startColumn: 1)
                }
            },
            new object[]
            {
                "set-variable", "set-variable-one-error", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "set-variable", startLine: 4, startColumn: 1)
                }
            },
            new object[]
            {
                "set-variable", "set-variable-two-errors", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "set-variable", startLine: 2, startColumn: 1),
                    new RuleViolation(ruleName: "set-variable", startLine: 7, startColumn: 1)
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
