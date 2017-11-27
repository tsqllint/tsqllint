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
            // WHERE N'a' + N'b' = N'ab' mixed variations
            new object[]
            {
                "concat-strings", "concat-strings-raw-where-error",  typeof(ConcatStringsRule), new List<RuleViolation>
                {
                    new RuleViolation("concat-strings", 3, 14),
                    new RuleViolation("concat-strings", 7, 14),
                    new RuleViolation("concat-strings", 10, 7),
                    new RuleViolation("concat-strings", 13, 7),
                    new RuleViolation("concat-strings", 16, 7),
                    new RuleViolation("concat-strings", 19, 7),
                    new RuleViolation("concat-strings", 22, 7),
                    new RuleViolation("concat-strings", 25, 7),
                    new RuleViolation("concat-strings", 28, 7),
                    new RuleViolation("concat-strings", 28, 15),
                    new RuleViolation("concat-strings", 31, 7),
                    new RuleViolation("concat-strings", 31, 15),
                    new RuleViolation("concat-strings", 34, 7),
                    new RuleViolation("concat-strings", 37, 7),
                    new RuleViolation("concat-strings", 37, 14),
                    new RuleViolation("concat-strings", 40, 7),
                    new RuleViolation("concat-strings", 40, 14),
                    new RuleViolation("concat-strings", 43, 7),
                    new RuleViolation("concat-strings", 46, 7),
                    new RuleViolation("concat-strings", 46, 15),
                    new RuleViolation("concat-strings", 49, 7),
                    new RuleViolation("concat-strings", 49, 15),
                    new RuleViolation("concat-strings", 52, 7) //TODO: more than one issue if comparison and concat have issues 
                }
            },
            // JOIN c.name = N'a' + N'b'; mixed variations
            new object[]
            {
                "concat-strings", "concat-strings-raw-join-error",  typeof(ConcatStringsRule), new List<RuleViolation>
                {
                    new RuleViolation("concat-strings", 5, 15),
                    new RuleViolation("concat-strings", 11, 15)
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
