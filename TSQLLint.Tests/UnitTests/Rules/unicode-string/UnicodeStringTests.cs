using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class UnicodeStringTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "unicode-string", "unicode-string-no-error", typeof(UnicodeStringRule), new List<RuleViolation>()
            },
            new object[]
            {
                "unicode-string", "unicode-string-error", typeof(UnicodeStringRule), new List<RuleViolation>
                {
                    new RuleViolation("unicode-string", 7, 8),
                    new RuleViolation("unicode-string", 8, 8),
                    new RuleViolation("unicode-string", 9, 8),
                    new RuleViolation("unicode-string", 10, 8),
                    new RuleViolation("unicode-string", 12, 14),
                    new RuleViolation("unicode-string", 13, 10)
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
