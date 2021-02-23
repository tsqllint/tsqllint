using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class CaseSensitiveVariablesRuleTests
    {
        private const string RuleName = "case-sensitive-variables";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "case-sensitive-variables-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "case-sensitive-variables-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 5, 8)
                }
            },
            new object[]
            {
                "case-sensitive-variables-two-errors", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 4, 8),
                    new RuleViolation(RuleName, 5, 8)
                }
            },
            new object[]
            {
                "case-sensitive-variables-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 4, 8),
                    new RuleViolation(RuleName, 5, 8)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('DECLARE @VariableName INT;
                    SELECT @VariableName = 1;');",
                new List<RuleViolation>()
            },
            new object[]
            {
                @"EXEC('DECLARE @VariableName INT;
                    SELECT @variableName = 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 28),
                }
            },
            new object[]
            {
                @"EXECUTE ('DECLARE @VariableName INT;
                    SELECT @variableName = 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 28),
                }
            },
            new object[]
            {
                @"EXEC('DECLARE @VariableName INT,
                    @SomeOtherVariable INT;

                SELECT @VariableName = 1;
                SELECT @someOtherVariable = 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 5, 24),
                }
            },
            new object[]
            {
                @"EXEC('DECLARE @VariableName INT, @SomeOtherVariable INT;

                SELECT @VariableName = 1; SELECT @someOtherVariable = 1;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 50),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName,  typeof(CaseSensitiveVariablesRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(CaseSensitiveVariablesRule), sql, expectedVioalations);
        }
    }
}
