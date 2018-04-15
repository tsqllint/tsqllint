using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DataCompressionRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "data-compression", "data-compression-no-error",  typeof(DataCompressionOptionRule), new List<RuleViolation>()
            },
            new object[]
            {
                "data-compression", "data-compression-one-error", typeof(DataCompressionOptionRule), new List<RuleViolation>
                {
                    new RuleViolation("data-compression", 1, 1)
                }
            },
            new object[]
            {
                "data-compression", "data-compression-two-errors", typeof(DataCompressionOptionRule), new List<RuleViolation>
                {
                    new RuleViolation("data-compression", 1, 1),
                    new RuleViolation("data-compression", 5, 1)
                }
            },
            new object[]
            {
                "data-compression", "data-compression-one-error-mixed-state", typeof(DataCompressionOptionRule), new List<RuleViolation>
                {
                    new RuleViolation("data-compression", 6, 1)
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
