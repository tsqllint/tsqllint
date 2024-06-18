using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class DuplicateEmptyLineRuleTests
    {
        private const string RuleName = "duplicate-empty-line";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "duplicate-empty-line-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "duplicate-empty-line-one-error", new List<RuleViolation>
                {
                     new RuleViolation(RuleName, 3, 1)
                }
            },
            new object[]
            {
                "duplicate-empty-line-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 1),
                    new RuleViolation(RuleName, 6, 1),
                    new RuleViolation(RuleName, 7, 1)
                }
            },
            new object[]
            {
                "duplicate-empty-line-EOF-error", new List<RuleViolation>
                {
                     new RuleViolation(RuleName, 7, 1)
                }
            },
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(DuplicateEmptyLineRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestRuleWithFix(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTestWithFix(RuleName, testFileName, typeof(DuplicateEmptyLineRule));
        }
    }
}
