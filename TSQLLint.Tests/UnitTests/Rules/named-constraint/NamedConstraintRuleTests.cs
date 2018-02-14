using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class NamedConstraintRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "named-constraint", "named-constraint-no-error",  typeof(NamedContraintRule), new List<RuleViolation>()
            },
            new object[]
            {
                "named-constraint", "named-constraint-one-error",  typeof(NamedContraintRule), new List<RuleViolation>
                {
                    new RuleViolation("named-constraint", 7, 1)
                }
            },
            new object[]
            {
                "named-constraint", "named-constraint-multi-error",  typeof(NamedContraintRule), new List<RuleViolation>
                {
                    new RuleViolation("named-constraint", 9, 1),
                    new RuleViolation("named-constraint", 16, 1),
                    new RuleViolation("named-constraint", 23, 1),
                    new RuleViolation("named-constraint", 30, 1),
                    new RuleViolation("named-constraint", 39, 1)
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
