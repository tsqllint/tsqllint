using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DuplicateGoRuleTests
    {
        private const string RuleName = "duplicate-go";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "duplicate-go-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "duplicate-go-one-error", new List<RuleViolation>
                {
                     new RuleViolation(RuleName, 3, 1)
                }
            },
            new object[]
            {
                "duplicate-go-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 1),
                    new RuleViolation(RuleName, 7, 1),
                    new RuleViolation(RuleName, 8, 1)
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(DuplicateGoRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(DuplicateGoRule));
        }
    }
}
