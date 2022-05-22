using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class CountStarRuleTests
    {
        private const string RuleName = "count-star";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "count-star-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "count-star-one-error", new List<RuleViolation>
                {
                    new RuleViolation("count-star", 1, 8)
                }
            },
            new object[]
            {
                "count-star-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("count-star", 1, 8),
                    new RuleViolation("count-star", 3, 8)
                }
            },
            new object[]
            {
                "count-star-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("count-star", 3, 12)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT COUNT(*) FROM FOO;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 14),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT COUNT(*) FROM FOO;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 28),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(CountStarRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(CountStarRule), sql, expectedViolations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(CountStarRule));
        }
    }
}
