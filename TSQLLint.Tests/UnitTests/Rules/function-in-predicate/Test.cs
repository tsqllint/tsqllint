using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class FunctionInPredicateTests
    {
        private static readonly object[] testCases =
        {
            new object[]
            {
                "function-in-predicate", "function-in-predicate-where",  typeof(FunctionInPredicate), new List<RuleViolation>()
                {
                    new RuleViolation("function-in-predicate", 1, 25)
                }
            },
            new object[]
            {
                "function-in-predicate", "function-in-predicate-from",  typeof(FunctionInPredicate), new List<RuleViolation>()
                {
                   new RuleViolation("function-in-predicate", 1, 37)
                }
            },
            new object[]
            {
                 "function-in-predicate", "function-in-predicate-update",  typeof(FunctionInPredicate), new List<RuleViolation>()
            },
            new object[]
            {
                 "function-in-predicate", "function-in-predicate-select",  typeof(FunctionInPredicate), new List<RuleViolation>()
            },
            new object[]
            {
                 "function-in-predicate", "function-in-predicate-check-constraint",  typeof(FunctionInPredicate), new List<RuleViolation>()
            }
        };

        [Test, TestCaseSource(nameof(testCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
