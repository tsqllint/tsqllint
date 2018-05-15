using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DataCompressionRuleTests
    {
        private const string RuleName = "data-compression";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "data-compression-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "data-compression-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "data-compression-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 5, 1)
                }
            },
            new object[]
            {
                "data-compression-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 6, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXECUTE('CREATE TABLE MyTable 
	                (ID INT, 
	                Name nvarchar(50))');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 10),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(DataCompressionOptionRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(DataCompressionOptionRule), sql, expectedVioalations);
        }
    }
}
