using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class JoinKeywordRuleTests
    {
        private const string RuleName = "join-keyword";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "join-keyword-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "join-keyword-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 15)
                }
            },
            new object[]
            {
                "join-keyword-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 15),
                    new RuleViolation(RuleName, 3, 15)
                }
            },
            new object[]
            {
                "join-keyword-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 19)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT FOO.ID FROM FOO, BAR WHERE FOO.ID = BAR.ID;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 21),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT FOO.ID FROM FOO, BAR WHERE FOO.ID = BAR.ID;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 35),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(JoinKeywordRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(JoinKeywordRule), sql, expectedViolations);
        }
    }
}
