using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class PredicateAlwaysTrueRuleTests
    {
        private static readonly object[] testCases =
        {
            new object[]
            {
                "predicate-is-always-true", "predicate-always-true-no-error",  typeof(PredicateIsAlwaysTrue), new List<RuleViolation>()
            },
            new object[]
            {
                "predicate-is-always-true", "predicate-always-true-same-columns",  typeof(PredicateIsAlwaysTrue), new List<RuleViolation>()
                {
                    new RuleViolation("predicate-is-always-true", 1, 25)
                }
            },
            new object[]
            {
                "predicate-is-always-true", "predicate-always-true-same-columns-with-alias",  typeof(PredicateIsAlwaysTrue), new List<RuleViolation>()
                {
                    new RuleViolation("predicate-is-always-true", 1, 27)
                }
            },
            new object[]
            {
                "predicate-is-always-true", "predicate-always-true-same-constants",  typeof(PredicateIsAlwaysTrue), new List<RuleViolation>()
                {
                    new RuleViolation("predicate-is-always-true", 1, 25)
                }
            },
            new object[]
            {
                "predicate-is-always-true", "predicate-always-true-join-same-constants",  typeof(PredicateIsAlwaysTrue), new List<RuleViolation>()
                {
                    new RuleViolation("predicate-is-always-true", 1, 37)
                }
            },
        };

        [Test, TestCaseSource(nameof(testCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
