using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class CrossDatabaseRuleTests
    {
        private static readonly object[] testCases =
        {
            new object[]
            {
                "cross-database", "cross-database-no-error",  typeof(CrossDatabaseRule), new List<RuleViolation>()
            },
            new object[]
            {
                "cross-database", "cross-database-one-error",  typeof(CrossDatabaseRule), new List<RuleViolation>()
                {
                    new RuleViolation(ruleName: "cross-database", startLine: 1, startColumn: 17)
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
