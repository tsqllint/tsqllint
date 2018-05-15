using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class ConditionalBeginEndRuleTests
    {
        private const string RuleName = "conditional-begin-end";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "conditional-begin-end-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "conditional-begin-end-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 1),
                    new RuleViolation(RuleName, 4, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 6, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('IF(1 = 1)
                            SELECT 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXECUTE ('IF(1 = 1)
                                SELECT 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 11),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName,  typeof(ConditionalBeginEndRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(ConditionalBeginEndRule), sql, expectedVioalations);
        }
    }
}
