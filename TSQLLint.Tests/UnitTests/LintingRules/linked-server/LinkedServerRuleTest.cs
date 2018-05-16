using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class LinkedServerRuleTest
    {
        private const string RuleName = "linked-server";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "linked-server-one-error", new List<RuleViolation>
                {
                    new RuleViolation("linked-server", 1, 17)
                }
            },
            new object[]
            {
                "linked-server-no-error", new List<RuleViolation>()
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT Foo FROM MyServer.MyDatabase.MySchema.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 23),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT Foo FROM MyServer.MyDatabase.MySchema.MyTable;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 37),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(LinkedServerRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(LinkedServerRule), sql, expectedVioalations);
        }
    }
}
