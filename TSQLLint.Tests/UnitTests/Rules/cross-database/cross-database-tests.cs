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
                "cross-database", "cross-database-one-error",  typeof(CrossDatabaseRule), new List<RuleViolation>
                {
                    new RuleViolation("cross-database", 1, 17)
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
