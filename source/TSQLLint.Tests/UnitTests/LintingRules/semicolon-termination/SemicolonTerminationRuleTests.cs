using System;
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
                  new RuleViolation(RuleName, 1, 18)
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
                  new RuleViolation(RuleName, 2, 24),
                  new RuleViolation(RuleName, 3, 28),
                  new RuleViolation(RuleName, 4, 36)
              }
          },
          new object[]
          {
              "semicolon-termination-multiple-errors", new List<RuleViolation>
              {
                  new RuleViolation(RuleName, 1, 20),
                  new RuleViolation(RuleName, 4, 13),
                  new RuleViolation(RuleName, 8, 10),
                  new RuleViolation(RuleName, 12, 47),
                  new RuleViolation(RuleName, 14, 29),
                  new RuleViolation(RuleName, 19, 47),
                  new RuleViolation(RuleName, 29, 36)
              }
          },
          new object[]
          {
              "semicolon-termination-one-error-mixed-state", new List<RuleViolation>
              {
                  new RuleViolation(RuleName, 1, 20)
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
                  new RuleViolation(RuleName, 3, 25)
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
                    new RuleViolation("semicolon-termination", 1, 15),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1');",
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 1, 18),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1')", // inner and outer statements missing semicolon
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 1, 18),
                    new RuleViolation("semicolon-termination", 1, 20),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT 1');",
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 2, 29),
                }
            },
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(SemicolonTerminationRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(SemicolonTerminationRule), sql, expectedVioalations);
        }
    }
}
