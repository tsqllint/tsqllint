using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DeleteWhereRuleTests
    {
        private const string RuleName = "delete-where";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "delete-where-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "delete-where-one-error", new List<RuleViolation>
                {
                     new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "delete-where-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 3, 1)
                }
            },
            new object[]
            {
                "delete-where-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 3, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('DELETE FROM dbo.MyTable
                    WHERE ID = 100;');",
                new List<RuleViolation>()
            },
            new object[]
            {
                @"EXEC('DELETE FROM dbo.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXECUTE ('DELETE FROM dbo.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 11),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1
                    DELETE FROM dbo.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1; DELETE FROM dbo.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 17),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName,  typeof(DeleteWhereRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(DeleteWhereRule), sql, expectedVioalations);
        }
    }
}