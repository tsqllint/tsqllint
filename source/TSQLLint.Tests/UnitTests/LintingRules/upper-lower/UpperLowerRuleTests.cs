using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class UpperLowerRuleTests
    {
        private const string RuleName = "upper-lower";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "upper-lower-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "upper-lower-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "upper-lower-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT * FROM Foo WHERE Status = lower(@Status)');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                        SELECT * FROM Foo 
                        WHERE Status = lower(@Status)');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 25),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(UpperLowerRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(UpperLowerRule), sql, expectedVioalations);
        }
    }
}
