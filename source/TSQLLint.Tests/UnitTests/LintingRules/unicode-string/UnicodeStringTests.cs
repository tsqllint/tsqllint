using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class UnicodeStringTests
    {
        private const string RuleName = "unicode-string";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "unicode-string-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "unicode-string-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 7, 8),
                    new RuleViolation(RuleName, 8, 8),
                    new RuleViolation(RuleName, 9, 8),
                    new RuleViolation(RuleName, 10, 8),
                    new RuleViolation(RuleName, 12, 14),
                    new RuleViolation(RuleName, 13, 10)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT ''I am テ in Japanese''');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 6)
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT ''I am テ in Japanese''');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 6)
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(UnicodeStringRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(UnicodeStringRule), sql, expectedVioalations);
        }
    }
}
