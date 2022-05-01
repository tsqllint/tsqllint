using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetAnsiRuleTests
    {
        private const string RuleName = "set-ansi";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-ansi-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "set-ansi-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "set-ansi-on-off-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "set-ansi-on-off-no-error", new List<RuleViolation>()
            }
        };

        // set-ansi is blacklistd and should not be reported against dynamic sql
        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"SET ANSI_NULLS ON;
                  EXEC('SET QUOTED_IDENTIFIER ON;
                  SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                  SET NOCOUNT ON;
                  SELECT * FROM FOO;');",
                new List<RuleViolation>()
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SetAnsiNullsRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SetAnsiNullsRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(SetAnsiNullsRule));
        }
    }
}
