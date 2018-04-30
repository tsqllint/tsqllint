using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class ConditionalBeginEndRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "conditional-begin-end", "conditional-begin-end-no-error",  typeof(ConditionalBeginEndRule), new List<RuleViolation>()
            },
            new object[]
            {
                "conditional-begin-end", "conditional-begin-end-one-error", typeof(ConditionalBeginEndRule), new List<RuleViolation>
                {
                    new RuleViolation("conditional-begin-end", 1, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end", "conditional-begin-end-two-errors", typeof(ConditionalBeginEndRule), new List<RuleViolation>
                {
                    new RuleViolation("conditional-begin-end", 1, 1),
                    new RuleViolation("conditional-begin-end", 4, 1)
                }
            },
            new object[]
            {
                "conditional-begin-end", "conditional-begin-end-one-error-mixed-state", typeof(ConditionalBeginEndRule), new List<RuleViolation>
                {
                    new RuleViolation("conditional-begin-end", 6, 1)
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
