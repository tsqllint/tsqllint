using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SchemaQualifyRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "schema-qualify", "schema-qualify-no-error",  typeof(SchemaQualifyRule), new List<RuleViolation>()
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-one-error", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 1, 17)
                }
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-two-errors", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 1, 17),
                    new RuleViolation("schema-qualify", 2, 17)
                }
            },
            new object[]
            {
                "schema-qualify", "schema-qualify-one-error-mixed-state", typeof(SchemaQualifyRule), new List<RuleViolation>
                {
                    new RuleViolation("schema-qualify", 3, 21)
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
