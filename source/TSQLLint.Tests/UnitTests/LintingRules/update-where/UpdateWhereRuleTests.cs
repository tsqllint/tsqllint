using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class UpdateWhereRuleTests
    {
        private const string RuleName = "update-where";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "update-where-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "update-where-one-error", new List<RuleViolation>
                {
                     new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "update-where-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 4, 1)
                }
            },
            new object[]
            {
                "update-where-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 4, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('UPDATE dbo.MyTable
                    SET TITLE = ''TEST''
                    WHERE ID = 100;');",
                new List<RuleViolation>()
            },
            new object[]
            {
                @"EXEC('UPDATE dbo.MyTable
                    SET TITLE = ''TEST'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXECUTE ('UPDATE dbo.MyTable
                    SET TITLE = ''TEST'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 11),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1
                    UPDATE dbo.MyTable
                        SET TITLE = ''TEST'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1; UPDATE dbo.MyTable
                    SET TITLE = ''TEST'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 17),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName,  typeof(UpdateWhereRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(UpdateWhereRule), sql, expectedVioalations);
        }
    }
}