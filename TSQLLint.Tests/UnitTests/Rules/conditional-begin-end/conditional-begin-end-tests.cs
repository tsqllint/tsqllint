using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class ConditionalBeginEndRuleTests
    {
        private static readonly object[] testCases = 
        {
          new object[]
          {
              "conditional-begin-end", "conditional-begin-end-no-error",  typeof(ConditionalBeginEndRule), new List<RuleViolation>()
          },
          new object[] 
          {
              "conditional-begin-end", "conditional-begin-end-one-error", typeof(ConditionalBeginEndRule), new List<RuleViolation>
          {
                  new RuleViolation(ruleName: "conditional-begin-end", startLine: 1, startColumn: 1)
              }
          },
          new object[] 
          {
              "conditional-begin-end", "conditional-begin-end-two-errors", typeof(ConditionalBeginEndRule), new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "conditional-begin-end", startLine: 1, startColumn: 1),
                  new RuleViolation(ruleName: "conditional-begin-end", startLine: 4, startColumn: 1)
              }
          },
          new object[] 
          {
              "conditional-begin-end", "conditional-begin-end-one-error-mixed-state", typeof(ConditionalBeginEndRule), new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "conditional-begin-end", startLine: 6, startColumn: 1)
              }
          }
        };
        
        [Test, TestCaseSource("testCases")]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
