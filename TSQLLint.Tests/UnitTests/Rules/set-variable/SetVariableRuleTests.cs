using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class SetVariableRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-variable", "set-variable-no-error",  typeof(SetVariableRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-variable", "set-variable-one-error-mixed-state", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation("set-variable", 6, 1)
                }
            },
            new object[]
            {
                "set-variable", "set-variable-one-error", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation("set-variable", 4, 1)
                }
            },
            new object[]
            {
                "set-variable", "set-variable-two-errors", typeof(SetVariableRule), new List<RuleViolation>
                {
                    new RuleViolation("set-variable", 2, 1),
                    new RuleViolation("set-variable", 7, 1)
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
