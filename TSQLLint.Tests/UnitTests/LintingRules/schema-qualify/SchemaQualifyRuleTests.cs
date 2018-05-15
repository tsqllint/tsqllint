using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SchemaQualifyRuleTests
    {
        private const string RuleName = "schema-qualify";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "schema-qualify-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "schema-qualify-one-error", new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 1, 17)
                }
            },
            new object[]
            {
                "schema-qualify-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 1, 17),
                    new RuleViolation("schema-qualify", 2, 17)
                }
            },
            new object[]
            {
                "schema-qualify-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 3, 21)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT FOO FROM BAR;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 23),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT FOO FROM BAR;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 37),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SchemaQualifyRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SchemaQualifyRule), sql, expectedVioalations);
        }
    }
}
