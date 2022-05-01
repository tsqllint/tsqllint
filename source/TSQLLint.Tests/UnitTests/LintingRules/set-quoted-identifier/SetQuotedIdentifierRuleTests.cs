using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetQuotedIdentifierRuleTests
    {
        private const string RuleName = "set-quoted-identifier";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-quoted-identifier-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "set-quoted-identifier-one-error", new List<RuleViolation>
                {
                    new RuleViolation("set-quoted-identifier", 1, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"SET QUOTED_IDENTIFIER ON;
                    EXEC('SET ANSI_NULLS ON;
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED  
                    SET NOCOUNT ON;
                    SELECT FOO FROM BAR;');",
                new List<RuleViolation>()
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SetQuotedIdentifierRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SetQuotedIdentifierRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
        #pragma warning disable IDE0060 // Remove unused parameter
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        #pragma warning restore IDE0060 // Remove unused parameter
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(SetQuotedIdentifierRule));
        }
    }
}
