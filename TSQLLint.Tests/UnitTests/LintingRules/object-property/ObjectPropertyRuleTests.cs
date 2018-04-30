using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class ObjectPropertyRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "object-property", "object-property-no-error",  typeof(ObjectPropertyRule), new List<RuleViolation>()
            },
            new object[]
            {
                "object-property", "object-property-one-error", typeof(ObjectPropertyRule), new List<RuleViolation>
                {
                    new RuleViolation("object-property", 3, 7)
                }
            },
            new object[]
            {
                "object-property", "object-property-two-errors", typeof(ObjectPropertyRule), new List<RuleViolation>
                {
                    new RuleViolation("object-property", 3, 7),
                    new RuleViolation("object-property", 8, 7)
                }
            },
            new object[]
            {
                "object-property", "object-property-one-error-mixed-state", typeof(ObjectPropertyRule), new List<RuleViolation>
                {
                    new RuleViolation("object-property", 5, 7)
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
