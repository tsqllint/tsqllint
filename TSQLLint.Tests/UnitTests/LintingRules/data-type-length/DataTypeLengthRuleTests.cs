using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DataTypeLengthRuleTests
    {
        private const string RuleName = "data-type-length";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "data-type-length-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "data-type-length-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 19)
                }
            },

            new object[]
            {
                "data-type-length-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 7, 15)
                }
            },
            new object[]
            {
                "data-type-length-all-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2,  17),
                    new RuleViolation(RuleName, 3,  20),
                    new RuleViolation(RuleName, 4,  23),
                    new RuleViolation(RuleName, 5,  19),
                    new RuleViolation(RuleName, 6,  20),
                    new RuleViolation(RuleName, 7,  22),
                    new RuleViolation(RuleName, 8,  22),
                    new RuleViolation(RuleName, 9,  22),
                    new RuleViolation(RuleName, 10, 19)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('CREATE TABLE MyTable 
                    (ID INT, 
                     Name nvarchar);');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 35),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(DataTypeLengthRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(DataTypeLengthRule), sql, expectedVioalations);
        }
    }
}
