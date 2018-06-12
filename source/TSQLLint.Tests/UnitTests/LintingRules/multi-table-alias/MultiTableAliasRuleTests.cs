using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class MultiTableAliasRuleTests
    {
        private const string RuleName = "multi-table-alias";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "multi-table-alias-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "multi-table-alias-one-error-with-tabs", new List<RuleViolation>
                {
                    new RuleViolation("multi-table-alias", 2, 10)
                }
            },
            new object[]
            {
                "multi-table-alias-one-error-with-spaces", new List<RuleViolation>
                {
                    new RuleViolation("multi-table-alias", 2, 10)
                }
            },
            new object[]
            {
                "multi-table-alias-multiple-errors-with-tabs", new List<RuleViolation>
                {
                    new RuleViolation("multi-table-alias", 2, 6),
                    new RuleViolation("multi-table-alias", 3, 6),
                    new RuleViolation("multi-table-alias", 5, 6),
                    new RuleViolation("multi-table-alias", 14, 6)
                }
            },
            new object[]
            {
                "multi-table-alias-multiple-errors-with-spaces", new List<RuleViolation>
                {
                    new RuleViolation("multi-table-alias", 2, 6),
                    new RuleViolation("multi-table-alias", 3, 6),
                    new RuleViolation("multi-table-alias", 5, 6)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT Name, v.Name FROM Purchasing.Product JOIN Purchasing.ProductVendor pv ON ProductID = pv.ProductID JOIN Purchasing.Vendor v ON pv.BusinessEntityID = v.BusinessEntityID WHERE ProductSubcategoryID = 15 ORDER BY v.Name;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 32),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT Name, v.Name
                        FROM Purchasing.Product
                    JOIN Purchasing.ProductVendor pv
                        ON ProductID = pv.ProductID
                    JOIN Purchasing.Vendor v
                        ON pv.BusinessEntityID = v.BusinessEntityID
                    WHERE ProductSubcategoryID = 15
                    ORDER BY v.Name;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 3, 30),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(MultiTableAliasRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(MultiTableAliasRule), sql, expectedVioalations);
        }
    }
}
