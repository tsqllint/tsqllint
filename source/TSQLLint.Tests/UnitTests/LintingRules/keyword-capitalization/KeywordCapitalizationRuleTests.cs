using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class KeywordCapitalizationRuleTests
    {
        private const string RuleName = "keyword-capitalization";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "keyword-capitalization-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "keyword-capitalization-one-error", new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 1, 1)
                }
            },
            new object[]
            {
                "keyword-capitalization-multiple-errors-tabs", new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 1, 1),
                    new RuleViolation("keyword-capitalization", 1, 8),
                    new RuleViolation("keyword-capitalization", 3, 20),
                    new RuleViolation("keyword-capitalization", 3, 24),
                    new RuleViolation("keyword-capitalization", 10, 8)
                }
            },
            new object[]
            {
                "keyword-capitalization-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("keyword-capitalization", 2, 10)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('select * FROM foo;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                    select * FROM foo;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(KeywordCapitalizationRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(KeywordCapitalizationRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
#pragma warning disable IDE0060 // Remove unused parameter
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(KeywordCapitalizationRule));
        }
    }
}
