using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class PositionalInsertRuleTests
    {
        private const string RuleName = "positional-insert";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "positional-insert-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "positional-insert-one-error", new List<RuleViolation>
                {
                    new RuleViolation("positional-insert", 1, 1)
                }
            },
            new object[]
            {
                "positional-insert-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("positional-insert", 1, 1),
                    new RuleViolation("positional-insert", 3, 1)
                }
            },
            new object[]
            {
                "positional-insert-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("positional-insert", 3, 5)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('INSERT INTO FOO VALUES (1, 2);');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 14),
                }
            },
            new object[]
            {
                @"EXEC('
                    INSERT INTO FOO VALUES (1, 2);');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 28),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(PositionalInsertRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(PositionalInsertRule), sql, expectedViolations);
        }
    }
}
