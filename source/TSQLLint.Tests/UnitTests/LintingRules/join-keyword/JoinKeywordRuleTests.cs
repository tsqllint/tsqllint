using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class JoinKeywordRuleTests
    {
        private const string RuleName = "join-keyword";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "join-keyword-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "join-keyword-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 15)
                }
            },
            new object[]
            {
                "join-keyword-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 15),
                    new RuleViolation(RuleName, 3, 15)
                }
            },
            new object[]
            {
                "join-keyword-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 19)
                }
            },
            new object[]
            {
                // issue #332: a comma join mixed with an explicit join must still
                // be flagged regardless of order -- "FROM FOO, BAR INNER JOIN BAZ"
                // and "FROM FOO INNER JOIN BAR ..., BAZ" were both missed by the
                // original per-element check.
                "join-keyword-mixed-comma-and-explicit", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 15),
                    new RuleViolation(RuleName, 2, 15)
                }
            },
            new object[]
            {
                // chained explicit joins and CROSS APPLY are not comma joins
                "join-keyword-no-error-explicit-joins", new List<RuleViolation>()
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT FOO.ID FROM FOO, BAR WHERE FOO.ID = BAR.ID;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 21),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT FOO.ID FROM FOO, BAR WHERE FOO.ID = BAR.ID;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 35),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(JoinKeywordRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(JoinKeywordRule), sql, expectedViolations);
        }
    }
}
