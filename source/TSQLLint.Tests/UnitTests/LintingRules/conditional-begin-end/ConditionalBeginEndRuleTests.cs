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
                    new (RuleName, 1, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end-two-errors", new List<RuleViolation>
                {
                    new (RuleName, 1, 1),
                    new (RuleName, 4, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end-multi-error", new List<RuleViolation>
                {
                    new (RuleName, 6, 1),
                    new (RuleName, 14, 5)
                }
            },
            new object[]
            {
                "conditional-else-begin-end-error", new List<RuleViolation>
                {
                    new (RuleName, 6, 5)
                }
            },
            new object[]
            {
                "conditional-if-else-begin-end-error", new List<RuleViolation>
                {
                    new (RuleName, 1, 1),
                }
            },
            new object[]
            {
                "conditional-begin-end-if-else-inline-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "conditional-begin-end-if-else-inline-errors", new List<RuleViolation>()
                {
                    new (RuleName, 1, 1),
                    new (RuleName, 2, 6),
                    new (RuleName, 3, 6),
                    new (RuleName, 4, 6)
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
                    new (RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXECUTE ('IF(1 = 1)
                                SELECT 1;');",
                new List<RuleViolation>
                {
                    new (RuleName, 1, 11),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1
                    IF(1 = 1)
                        SELECT 2;');",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 21),
                }
            },
            new object[]
            {
                @"EXEC('SELECT 1; IF(1 = 1)
                        SELECT 2;');",
                new List<RuleViolation>
                {
                    new (RuleName, 1, 17),
                }
            },
            new object[]
            {
                @"DECLARE @Sql NVARCHAR(400);
                    SELECT @Sql = 'IF(1 = 1)
                                       SELECT 1;';
                    EXEC (@Sql);",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 36),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(ConditionalBeginEndRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(ConditionalBeginEndRule), sql, expectedViolations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(ConditionalBeginEndRule));
        }
    }
}
