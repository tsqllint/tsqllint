using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SchemaQualifyRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "schema-qualify", "schema-qualify-no-error",  typeof(SchemaQualifyRule), new List<RuleViolation>()
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-one-error", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "schema-qualify", startLine: 1, startColumn: 17)
                }
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-two-errors", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "schema-qualify", startLine: 1, startColumn: 17),
                    new RuleViolation(ruleName: "schema-qualify", startLine: 2, startColumn: 17)
                }
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-one-error-mixed-state", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "schema-qualify", startLine: 3, startColumn: 21)
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
