using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public class ObjectPropertyRuleTests
    {
        private const string RuleName = "object-property";

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "object-property-no-error", new List<RuleViolation>()
            },
            new object[]
            {
                "object-property-one-error", new List<RuleViolation>
                {
                    new RuleViolation("object-property", 3, 7)
                }
            },
            new object[]
            {
                "object-property-two-errors", new List<RuleViolation>
                {
                    new RuleViolation("object-property", 3, 7),
                    new RuleViolation("object-property", 8, 7)
                }
            },
            new object[]
            {
                "object-property-one-error-mixed-state", new List<RuleViolation>
                {
                    new RuleViolation("object-property", 5, 7)
                }
            }
        };

        private static readonly object[] DynamicSqlTestCases =
        {
            new object[]
            {
                @"EXEC('SELECT name, object_id, type_desc FROM sys.objects WHERE OBJECTPROPERTY(object_id, N''SchemaId'') = SCHEMA_ID(N''Production'') ORDER BY type_desc, name;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 1, 64),
                }
            },
            new object[]
            {
                @"EXEC('
                    SELECT name, object_id, type_desc  
                    FROM sys.objects   
                    WHERE OBJECTPROPERTY(object_id, N''SchemaId'') = SCHEMA_ID(N''Production'')  
                    ORDER BY type_desc, name;');",
                new List<RuleViolation>
                {
                    new RuleViolation(RuleName, 4, 27),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void TestRule(string testFileName, List<RuleViolation> expectedRuleViolations)
        {
            RulesTestHelper.RunRulesTest(RuleName, testFileName, typeof(ObjectPropertyRule), expectedRuleViolations);
        }

        [TestCaseSource(nameof(DynamicSqlTestCases))]
        public void TestRuleWithDynamicSql(string sql, List<RuleViolation> expectedVioalations)
        {
            RulesTestHelper.RunDynamicSQLRulesTest(typeof(ObjectPropertyRule), sql, expectedVioalations);
        }
    }
}
