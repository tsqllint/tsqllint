using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class NonSargableRuleTests
    {
        private const string RuleName = "non-sargable";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "non-sargable-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "non-sargable-no-error-function-in-inner-join", new List<RuleViolation>()
            },
            new object[]
            {
                "non-sargable-one-error-where-clause", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 25)
                }
            },
            new object[]
            {
                "non-sargable-one-error-join-table", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 19)
                }
            },
            new object[]
            {
                "non-sargable-isnull-one-clause-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 7)
                }
            },
            new object[]
            {
                "non-sargable-isnull-multi-statement-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 6, 7)
                }
            },
            new object[]
            {
                "non-sargable-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 37),
                    new RuleViolation(RuleName, 1, 55),
                    new RuleViolation(RuleName, 3, 37),
                    new RuleViolation(RuleName, 5, 31),
                    new RuleViolation(RuleName, 7, 25),
                    new RuleViolation(RuleName, 9, 28),
                    new RuleViolation(RuleName, 11, 25),
                    new RuleViolation(RuleName, 13, 25),
                    new RuleViolation(RuleName, 15, 25),
                    new RuleViolation(RuleName, 17, 25),
                    new RuleViolation(RuleName, 19, 28),
                    new RuleViolation(RuleName, 21, 28),
                    new RuleViolation(RuleName, 29, 12),
                    new RuleViolation(RuleName, 37, 11),
                    new RuleViolation(RuleName, 45, 13),
                    new RuleViolation(RuleName, 54, 13)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT * FROM Foo WHERE UPPER(Foo.name) = ''FOO'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 31),
                }
            },
            new object[]
            {
                @"EXEC('SELECT *
                        FROM Foo
                        WHERE UPPER(Foo.name) = ''FOO'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 31),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(NonSargableRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(NonSargableRule), sql, expectedVioalations);
        }
    }
}
