using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DisallowCursorsRuleTests
    {
        private const string RuleName = "disallow-cursors";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "disallow-cursors-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "disallow-cursors-one-error", new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 3, 1)
                }
            },
            new object[]
            {
                "disallow-cursors-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 3, 1),
                    new RuleViolation("disallow-cursors", 4, 1)
                }
            },
            new object[]
            {
                "disallow-cursors-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("disallow-cursors", 4, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('OPEN some_cursor;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                    OPEN some_cursor;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(DisallowCursorRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(DisallowCursorRule), sql, expectedVioalations);
        }
    }
}
