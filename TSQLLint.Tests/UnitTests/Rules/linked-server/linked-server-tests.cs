using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class LinkedServerRuleTest
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "linked-server", "linked-server-one-error", typeof(LinkedServerRule), new List<RuleViolation>
                {
                    new RuleViolation("linked-server", 1, 17)
                }
            },
            new object[] { "linked-server", "linked-server-no-error", typeof(LinkedServerRule), new List<RuleViolation>() }
        };
        
        [Test, TestCaseSource(nameof(testCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
