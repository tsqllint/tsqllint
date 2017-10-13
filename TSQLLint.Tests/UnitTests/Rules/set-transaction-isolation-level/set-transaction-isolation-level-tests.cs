using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SetTransactionIsolationLevelRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "set-transaction-isolation-level", "set-transaction-isolation-level-no-error",  typeof(SetTransactionIsolationLevelRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-transaction-isolation-level", "set-transaction-isolation-level-one-error", typeof(SetTransactionIsolationLevelRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "set-transaction-isolation-level", startLine: 1, startColumn: 1)
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
