using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class InformationSchemaRuleTests
    {
        private const string RuleName = "information-schema";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "information-schema-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "information-schema-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 27)
                }
            },
            new object[]
            {
                "information-schema-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 5, 26),
                    new RuleViolation(RuleName, 2, 27)
                }
            },
            new object[]
            {
                "information-schema-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 27)
                }
            },
            new object[]
            {
                "information-schema-where-clause", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 30),
                    new RuleViolation(RuleName, 10, 26),
                    new RuleViolation(RuleName, 18, 26)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT TABLE_CATALOG FROM SomeDatabase.INFORMATION_SCHEMA.COLUMNS;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 33)
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT TABLE_CATALOG FROM SomeDatabase.INFORMATION_SCHEMA.COLUMNS;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 47)
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(InformationSchemaRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(InformationSchemaRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(InformationSchemaRule));
        }
    }
}
