using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetTransactionIsolationLevelRuleTests
    {
        private const string RuleName = "set-transaction-isolation-level";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-transaction-isolation-level-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "set-transaction-isolation-level-one-error", new List<RuleViolation>
                {
                    new RuleViolation("set-transaction-isolation-level", 1, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    EXEC('SET ANSI_NULLS ON;
                    SET NOCOUNT ON;
                    SET QUOTED_IDENTIFIER ON;
                    SELECT FOO FROM BAR;);",
                new List<RuleViolation>()
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SetTransactionIsolationLevelRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SetTransactionIsolationLevelRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(SetTransactionIsolationLevelRule));
        }
    }
}
