using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public class MultiTableAliasRuleTests
    {
        private static readonly object[] testCases = 
        {
            new object[]
            {
                "multi-table-alias", "multi-table-alias-no-error",  typeof(MultiTableAliasRule), new List<RuleViolation>()
            },
            new object[]
            {
                "multi-table-alias", "multi-table-alias-one-error-with-tabs", typeof(MultiTableAliasRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 2, startColumn: 10)
                }
            },
            new object[]
            {
                "multi-table-alias", "multi-table-alias-one-error-with-spaces", typeof(MultiTableAliasRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 2, startColumn: 10)
                }
            },
            new object[]
            {
                "multi-table-alias", "multi-table-alias-multiple-errors-with-tabs", typeof(MultiTableAliasRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 2, startColumn: 6),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 3, startColumn: 6),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 5, startColumn: 6),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 14, startColumn: 6)
                }
            },
            new object[]
            {
                "multi-table-alias", "multi-table-alias-multiple-errors-with-spaces", typeof(MultiTableAliasRule), new List<RuleViolation>
                {
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 2, startColumn: 6),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 3, startColumn: 6),
                    new RuleViolation(ruleName: "multi-table-alias", startLine: 5, startColumn: 6)
                }
            }
        };
        
        [Test, TestCaseSource("testCases")]
        public void TestRule(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(rule, testFileName, ruleType, expectedRuleViolations);
        }
    }
}
