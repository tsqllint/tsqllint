using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class KeywordCapitalizationRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "keyword-capitalization", "keyword-capitalization-no-error",  typeof(KeywordCapitalizationRule), new List<RuleViolation>()
            },
            new object[]
            {
                "keyword-capitalization", "keyword-capitalization-one-error", typeof(KeywordCapitalizationRule), new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 1, 1)
                }
            },
            new object[]
            {
                "keyword-capitalization", "keyword-capitalization-multiple-errors-tabs", typeof(KeywordCapitalizationRule), new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 1, 1),
                    new RuleViolation("keyword-capitalization", 1, 8),
                    new RuleViolation("keyword-capitalization", 3, 20),
                    new RuleViolation("keyword-capitalization", 3, 24)
                }
            },
            new object[]
            {
                "keyword-capitalization", "keyword-capitalization-one-error-mixed-state", typeof(KeywordCapitalizationRule), new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 2, 10)
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
