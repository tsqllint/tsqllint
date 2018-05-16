using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetNocountRuleTests
    {
        private const string RuleName = "set-nocount";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-nocount-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "set-nocount-no-error-ddl", new List<RuleViolation>()
            },
            new object[]
            {
                "set-nocount-one-error-rowset-action", new List<RuleViolation>
                {
                    new RuleViolation("set-nocount", 1, 1)
                }
            },
            new object[]
            {
                "set-nocount-no-rowset-action", new List<RuleViolation>()
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"SET NOCOUNT ON;
                EXEC('SET ANSI_NULLS ON;
                    SET QUOTED_IDENTIFIER ON;
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT FOO FROM BAR;');",
                new List<RuleViolation>()
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SetNoCountRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SetNoCountRule), sql, expectedVioalations);
        }
    }
}
