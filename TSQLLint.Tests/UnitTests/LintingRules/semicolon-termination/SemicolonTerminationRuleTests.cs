using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SemicolonTerminationRuleTests
    {
        private static readonly object[] TestCases =
        {
          new object[]
          {
              "semicolon-termination", "semicolon-termination-no-error",  typeof(SemicolonTerminationRule), new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-no-error-waitfor",  typeof(SemicolonTerminationRule), new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-one-error", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation("semicolon-termination", 1, 18)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-no-error-create-object", typeof(SemicolonTerminationRule), new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-multiple-errors-with-tab", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation("semicolon-termination", 2, 24),
                  new RuleViolation("semicolon-termination", 3, 28),
                  new RuleViolation("semicolon-termination", 4, 36)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-multiple-errors", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation("semicolon-termination", 1, 20),
                  new RuleViolation("semicolon-termination", 4, 13),
                  new RuleViolation("semicolon-termination", 8, 10),
                  new RuleViolation("semicolon-termination", 12, 47),
                  new RuleViolation("semicolon-termination", 14, 29),
                  new RuleViolation("semicolon-termination", 19, 47),
                  new RuleViolation("semicolon-termination", 29, 36)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-one-error-mixed-state", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation("semicolon-termination", 1, 20)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-try-catch-while",  typeof(SemicolonTerminationRule), new List<RuleViolation>()
          }
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
