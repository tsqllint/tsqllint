using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class NamedConstraintRuleTests
    {
        private const string RuleName = "named-constraint";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "named-constraint-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "named-constraint-one-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 7, 1)
                }
            },
            new object[]
            {
                "named-constraint-multi-error", new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 9, 1),
                    new RuleViolation(RuleName, 16, 1),
                    new RuleViolation(RuleName, 23, 1),
                    new RuleViolation(RuleName, 30, 1),
                    new RuleViolation(RuleName, 39, 1)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('CREATE TABLE #MyTable (ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, VALUE INT NOT NULL, ModifiedDate DATETIME NOT NULL CONSTRAINT DF_ModifiedDate DEFAULT GETDATE()) WITH(DATA_COMPRESSION = ROW);');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 7),
                }
            },
            new object[]
            {
                @"EXEC('
                CREATE TABLE #MyTable (
                  ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                  VALUE INT NOT NULL,
                  ModifiedDate DATETIME NOT NULL CONSTRAINT DF_ModifiedDate DEFAULT GETDATE()
                ) WITH(DATA_COMPRESSION = ROW);');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 2, 17),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(NamedConstraintRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedViolations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(NamedConstraintRule), sql, expectedViolations);
        }
    }
}
