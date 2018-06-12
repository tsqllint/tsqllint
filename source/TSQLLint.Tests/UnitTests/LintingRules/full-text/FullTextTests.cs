using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    [TestFixture]
    public class FullTextTests
    {
        private const string RuleName = "full-text";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "full-text-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "full-text-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("full-text", 6, 8)
                }
            },
            new object[]
            {
                "full-text-one-error", new List<RuleViolation>
                {
                    new RuleViolation("full-text", 4, 8),
                }
            },
            new object[]
            {
                "full-text-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("full-text", 4, 8),
                    new RuleViolation("full-text", 11, 7)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT Name, ListPrice FROM Production.Product WHERE ListPrice = 80.99 AND CONTAINS(Name, ''Mountain'')');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 82),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT Name, ListPrice
                    FROM Production.Product
                    WHERE ListPrice = 80.99
                       AND CONTAINS(Name, ''Mountain'')');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 5, 28),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(FullTextRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(FullTextRule), sql, expectedVioalations);
        }
    }
}
