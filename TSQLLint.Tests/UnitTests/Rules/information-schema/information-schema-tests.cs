using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class InformationSchemaRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "information-schema", "information-schema-no-error",  typeof(InformationSchemaRule), new List<RuleViolation>()
            },
            new object[]
            {
                "information-schema", "information-schema-one-error", typeof(InformationSchemaRule), new List<RuleViolation>
                {
                    new RuleViolation("information-schema", 2, 27)
                }
            },
            new object[]
            {
                "information-schema", "information-schema-two-errors", typeof(InformationSchemaRule), new List<RuleViolation>
                {
                    new RuleViolation("information-schema", 5, 26),
                    new RuleViolation("information-schema", 2, 27)
                }
            },
            new object[]
            {
                "information-schema", "information-schema-one-error-mixed-state", typeof(InformationSchemaRule), new List<RuleViolation>
                {
                    new RuleViolation("information-schema", 2, 27)
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
