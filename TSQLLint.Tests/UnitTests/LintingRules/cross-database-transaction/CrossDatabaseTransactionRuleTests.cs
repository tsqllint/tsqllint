using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class CrossDatabaseTransactionRuleTests
    {
        private const string RuleName = "cross-database-transaction";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "cross-database-transaction-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "cross-database-transaction-no-commit-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "cross-database-transaction-no-begintran-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "cross-database-transaction-no-error-single-line", new List<RuleViolation>()
            },
            new object[]
            {
                "cross-database-transaction-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "cross-database-transaction-multiple-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 6, 1)
                }
            },
            new object[]
            {
                "cross-database-transaction-nested-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 5, 5)
                }
            },
            new object[]
            {
                "cross-database-transaction-one-error-single-line", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('BEGIN TRANSACTION;
                    UPDATE DB1.dbo.Table1 SET Value = 1;
                    UPDATE DB2.dbo.Table2 SET Value = 1;
                COMMIT TRANSACTION;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                    BEGIN TRANSACTION;
                        UPDATE DB1.dbo.Table1 SET Value = 1;
                        UPDATE DB2.dbo.Table2 SET Value = 1;
                    COMMIT TRANSACTION;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(CrossDatabaseTransactionRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(CrossDatabaseTransactionRule), sql, expectedVioalations);
        }
    }
}
