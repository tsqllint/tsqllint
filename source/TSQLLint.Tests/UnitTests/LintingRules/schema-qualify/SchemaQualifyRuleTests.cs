using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SchemaQualifyRuleTests
    {
        private const string RuleName = "schema-qualify";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "schema-qualify-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "schema-qualify-one-error", new List<RuleViolation>
                {
                    new ("schema-qualify", 1, 17)
                }
            },
            new object[]
            {
                "schema-qualify-two-errors", new List<RuleViolation>
                {
                    new ("schema-qualify", 1, 17),
                    new ("schema-qualify", 2, 17)
                }
            },
            new object[]
            {
                "schema-qualify-one-error-mixed-state", new List<RuleViolation>
                {
                    new ("schema-qualify", 3, 21)
                }
            },
            new object[]
            {
                "schema-qualify-multi-error", new List<RuleViolation>
                {
                    new (RuleName, 1, 15),
                    new (RuleName, 3, 8),
                    new (RuleName, 5, 8),
                    new (RuleName, 7, 13),
                    new (RuleName, 9, 16),
                    new (RuleName, 11, 14),
                    new (RuleName, 13, 12),
                    new (RuleName, 15, 13),
                    new (RuleName, 17, 13)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT FOO FROM BAR;');",
                new List<RuleViolation>
                {
                    new (RuleName, 1, 23),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT FOO FROM BAR;');",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 37),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SchemaQualifyRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SchemaQualifyRule), sql, expectedViolations);
        }
    }
}
