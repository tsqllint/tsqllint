using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class PrintStatementRuleTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "print-statement", "print-statement-no-error",  typeof(PrintStatementRule), new List<RuleViolation>()
            },
            new object[]
            {
                "print-statement", "print-statement-one-error", typeof(PrintStatementRule), new List<RuleViolation>
                {
                    new RuleViolation("print-statement", 1, 1)
                }
            },
            new object[]
            {
                "print-statement", "print-statement-two-errors", typeof(PrintStatementRule), new List<RuleViolation>
                {
                    new RuleViolation("print-statement", 1, 1),
                    new RuleViolation("print-statement", 2, 1)
                }
            },
            new object[]
            {
                "print-statement", "print-statement-one-error-mixed-state", typeof(PrintStatementRule), new List<RuleViolation>
                {
                    new RuleViolation("print-statement", 3, 5)
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
