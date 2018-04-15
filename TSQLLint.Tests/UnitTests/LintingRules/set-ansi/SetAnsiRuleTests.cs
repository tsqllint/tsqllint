using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetAnsiRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-ansi", "set-ansi-no-error",  typeof(SetAnsiNullsRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-ansi", "set-ansi-one-error", typeof(SetAnsiNullsRule), new List<RuleViolation>
                {
                    new RuleViolation("set-ansi", 1, 1)
                }
            },
            new object[]
            {
                "set-ansi", "set-ansi-on-off-error", typeof(SetAnsiNullsRule), new List<RuleViolation>
                {
                    new RuleViolation("set-ansi", 1, 1)
                }
            },
            new object[]
            {
                "set-ansi", "set-ansi-on-off-no-error",  typeof(SetAnsiNullsRule), new List<RuleViolation>()
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
