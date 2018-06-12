using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SelectStarRuleTests
    {
        private const string RuleName = "select-star";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "select-star-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "select-star-one-error", new List<RuleViolation>
                {
                    new RuleViolation("select-star", 1, 8)
                }
            },
            new object[]
            {
                "select-star-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("select-star", 1, 8),
                    new RuleViolation("select-star", 3, 14)
                }
            },
            new object[]
            {
                "select-star-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("select-star", 3, 12)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT * FROM FOO;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 14),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT * FROM FOO;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 28),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SelectStarRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SelectStarRule), sql, expectedVioalations);
        }
    }
}
