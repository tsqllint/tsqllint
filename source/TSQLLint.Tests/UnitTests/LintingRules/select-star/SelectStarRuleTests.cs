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
                    new (RuleName, 1, 8)
                }
            },
            new object[]
            {
                "select-star-two-errors", new List<RuleViolation>
                {
                    new (RuleName, 1, 8),
                    new (RuleName, 3, 14)
                }
            },
            new object[]
            {
                "select-star-one-error-mixed-state", new List<RuleViolation>
                {
                    new (RuleName, 3, 12)
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
                    new (RuleName, 1, 14),
                }
            },
            new object[]
            {
                @"SELECT [Name] FROM dbo.MyTable;
                  EXEC(
                    'SELECT * FROM FOO;');",
                new List<RuleViolation>
                {
                    new (RuleName, 3, 29),
                }
            },
            new object[]
            {
                @"SELECT [Name] FROM dbo.MyTable;
                  EXEC('
                    SELECT * FROM FOO;');",
                new List<RuleViolation>
                {
                    new (RuleName, 3, 28),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT * FROM FOO;');",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 28),
                }
            },
            new object[]
            {
                @"DECLARE @Sql NVARCHAR(400);
                    SET @Sql = 'SELECT * FROM dbo.MyTable;';
                    EXEC (@Sql);",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 40),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SelectStarRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SelectStarRule), sql, expectedViolations);
        }
    }
}
