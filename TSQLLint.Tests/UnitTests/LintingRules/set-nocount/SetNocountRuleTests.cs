using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class SetNocountRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "set-nocount", "set-nocount-no-error",  typeof(SetNoCountRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-nocount", "set-nocount-no-error-ddl", typeof(SetNoCountRule), new List<RuleViolation>()
            },
            new object[]
            {
                "set-nocount", "set-nocount-one-error-rowset-action", typeof(SetNoCountRule), new List<RuleViolation>
                {
                    new RuleViolation("set-nocount", 1, 1)
                }
            },
            new object[]
            {
                "set-nocount", "set-nocount-no-rowset-action",  typeof(SetNoCountRule), new List<RuleViolation>()
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
