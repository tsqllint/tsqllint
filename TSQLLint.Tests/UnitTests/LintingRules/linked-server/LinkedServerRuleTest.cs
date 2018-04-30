using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class LinkedServerRuleTest
    {
        private static readonly object[] TestCases =
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

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
