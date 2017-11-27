using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class ConcatStringTests
    {
        private static readonly object[] TestCases =
        {
            //new object[]
            //{
            //    "concat-strings", "test",  typeof(ConcatStringsRule), new List<RuleViolation>()
            //},
            // varchar combinations allowed
            new object[]
            {
                "concat-strings", "concat-strings-raw-varchar-no-error",  typeof(ConcatStringsRule), new List<RuleViolation>()
            },
            // nvarchar combinations allowed
            new object[]
            {
                "concat-strings", "concat-strings-raw-nvarchar-no-error",  typeof(ConcatStringsRule), new List<RuleViolation>()
            },
            // SELECT 'a' + 'b' mixed variations
            new object[]
            {
                "concat-strings", "concat-strings-raw-concat-two-error",  typeof(ConcatStringsRule), new List<RuleViolation>
                {
                    new RuleViolation("concat-strings", 1, 8),
                    new RuleViolation("concat-strings", 3, 8),
                    new RuleViolation("concat-strings", 5, 8),
                    new RuleViolation("concat-strings", 8, 8),
                    new RuleViolation("concat-strings", 11, 8),
                    new RuleViolation("concat-strings", 14, 8)
                }
            },
            // SELECT 'a' + 'b' + 'c' mixed variations
            new object[]
            {
                "concat-strings", "concat-strings-raw-concat-three-error",  typeof(ConcatStringsRule), new List<RuleViolation>
                {
                    new RuleViolation("concat-strings", 1, 8),
                    new RuleViolation("concat-strings", 3, 8),
                    new RuleViolation("concat-strings", 5, 8),
                    new RuleViolation("concat-strings", 7, 8)
                }
            },
            // SELECT CASE WHEN 'a' + 'b' = 'ab' THEN 1 ELSE 0 END mixed variations
            new object[]
            {
                "concat-strings", "concat-strings-raw-case-error",  typeof(ConcatStringsRule), new List<RuleViolation>
                {
                    new RuleViolation("concat-strings", 1, 18),
                    new RuleViolation("concat-strings", 3, 18),
                    new RuleViolation("concat-strings", 5, 18),
                    new RuleViolation("concat-strings", 7, 18)
                }
            },
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
