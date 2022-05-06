using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetVariableRuleTests
    {
        private const string RuleName = "set-variable";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-variable-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "set-variable-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 6, 1)
                }
            },
            new object[]
            {
                "set-variable-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 4, 1)
                }
            },
            new object[]
            {
                "set-variable-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 1),
                    new RuleViolation(RuleName, 7, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SET @var1 = ''bar'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                    SET @var1 = ''bar'';');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 21),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SetVariableRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SetVariableRule), sql, expectedVioalations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(SetVariableRule));
        }
    }
}
