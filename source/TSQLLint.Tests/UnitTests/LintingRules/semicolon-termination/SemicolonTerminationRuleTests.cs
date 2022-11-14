using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SemicolonTerminationRuleTests
    {
        private const string RuleName = "semicolon-termination";

        private static readonly object[] TestCases =
        {
          new object[]
          {
              "semicolon-termination-no-error", new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination-no-error-waitfor", new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination-one-error", new List<RuleViolation>
              {
                  new (RuleName, 1, 18)
              }
          },
          new object[]
          {
               "semicolon-termination-no-error-create-object", new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination-multiple-errors-with-tab", new List<RuleViolation>
              {
                  new (RuleName, 2, 24),
                  new (RuleName, 3, 28),
                  new (RuleName, 4, 36)
              }
          },
          new object[]
          {
              "semicolon-termination-multiple-errors", new List<RuleViolation>
              {
                  new (RuleName, 1, 20),
                  new (RuleName, 4, 13),
                  new (RuleName, 8, 10),
                  new (RuleName, 12, 47),
                  new (RuleName, 14, 29),
                  new (RuleName, 19, 47),
                  new (RuleName, 29, 36)
              }
          },
          new object[]
          {
              "semicolon-termination-one-error-mixed-state", new List<RuleViolation>
              {
                  new (RuleName, 1, 20)
              }
          },
          new object[]
          {
              "semicolon-termination-try-catch-while", new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination-one-error-create-view", new List<RuleViolation>()
              {
                  new (RuleName, 3, 25)
              }
          },
          new object[]
          {
              "semicolon-termination-multiple-inline-errors", new List<RuleViolation>()
              {
                  new (RuleName, 1, 9),
                  new (RuleName, 1, 18)
              }
          }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT 1');",
                new List<RuleViolation>
                {
                    new ("semicolon-termination", 1, 15),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1');",
                new List<RuleViolation>
                {
                    new ("semicolon-termination", 1, 18),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1')", // inner and outer statements missing semicolon
                new List<RuleViolation>
                {
                    new (RuleName, 1, 18),
                    new (RuleName, 1, 20),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT 1');",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 29),
                }
            },
            new object[]
            {
                @"DECLARE @Sql NVARCHAR(400);
                    SET @Sql = 'CREATE PROCEDURE dbo.test AS BEGIN RETURN 0; END';
                    EXEC (@Sql);",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 81)
                }
            },
            new object[]
            {
                @"DECLARE @Sql NVARCHAR(400);
                    SELECT @Sql = 'CREATE PROCEDURE dbo.test AS BEGIN RETURN 0 END';
                    EXEC (@Sql);",
                new List<RuleViolation>
                {
                    new (RuleName, 2, 79),
                    new (RuleName, 2, 83)
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SemicolonTerminationRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SemicolonTerminationRule), sql, expectedViolations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(SemicolonTerminationRule));
        }
    }
}
