using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SemicolonTerminationRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
          {
              "semicolon-termination", "semicolon-termination-no-error",  typeof(SemicolonTerminationRule), new List<RuleViolation>()
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-one-error", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 1, startColumn: 18)
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
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 2, startColumn: 24),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 3, startColumn: 28),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 4, startColumn: 36)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-multiple-errors", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 1, startColumn: 20),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 4, startColumn: 13),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 12, startColumn: 47),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 14, startColumn: 29),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 19, startColumn: 47),
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 26, startColumn: 4)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-one-error-mixed-state", typeof(SemicolonTerminationRule), new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "semicolon-termination", startLine: 1, startColumn: 20)
              }
          },
          new object[]
          {
              "semicolon-termination", "semicolon-termination-try-catch-while",  typeof(SemicolonTerminationRule), new List<RuleViolation>()
          },
        };
        
        [Test, TestCaseSource("testCases")]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
