using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetTransactionIsolationLevelRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-transaction-isolation-level", "set-transaction-isolation-level-no-error",  typeof(SetTransactionIsolationLevelRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-transaction-isolation-level", "set-transaction-isolation-level-one-error", typeof(SetTransactionIsolationLevelRule), new List<RuleViolation>
                {
                    new RuleViolation("set-transaction-isolation-level", 1, 1)
                }
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
