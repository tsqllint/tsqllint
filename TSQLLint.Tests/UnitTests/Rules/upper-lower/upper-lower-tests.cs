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
                    new RuleViolation("upper-lower", 1, 1)
                }
            },
            new object[]
            {
                "upper-lower", "upper-lower-one-error-mixed-state", typeof(UpperLowerRule), new List<RuleViolation>
                {
                    new RuleViolation("upper-lower", 3, 1)
                }
            }
        };
        
        [Test, TestCaseSource(nameof(testCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
