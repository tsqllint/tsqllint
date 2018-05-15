using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class PrintStatementRuleTests
    {
        private const string RuleName = "print-statement";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                 "print-statement-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "print-statement-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "print-statement-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 2, 1)
                }
            },
            new object[]
            {
                "print-statement-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 5)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('PRINT ''Foo''');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                    PRINT ''Foo''');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(PrintStatementRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(PrintStatementRule), sql, expectedVioalations);
        }
    }
}
