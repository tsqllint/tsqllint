using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLINT_LIB;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.RuleViolations;
using TSQLLINT_LIB_TESTS.Integration_Tests;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.Rules
{
    public class SchemaQualifyTests
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [TestCase("schema-qualify-no-error",              0, Description = "one statement, one schema-qualify rule violation")]
        [TestCase("schema-qualify-one-error-mixed-state", 1, Description = "two statements, one schema-qualify rule violation")]
        [TestCase("schema-qualify-one-error",             1, Description = "one statement, one schema-qualify rule violation")]
        [TestCase("schema-qualify-two-errors",            2, Description = "two statements, two schema-qualify violations")]
        public void SelectStarRuleVisitorTests(string testFileName, int violationCount)
        {
            // arrange
            var sqlString = TestHelper.GetTestFile(string.Format("Rules\\schema-qualify\\test files\\{0}.sql", testFileName));

            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action <string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new SchemaQualifyRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);

            // act
            fragmentVisitor.VisistRule(textReader, visitor);

            // assert
            Assert.AreEqual(violationCount, ruleViolations.Count);

            foreach (var ruleViolation in ruleViolations)
            {
                Assert.AreEqual(ruleViolation.RuleName, "schema-qualify");
                Assert.AreEqual(ruleViolation.Text, "Schema qualify all object names");
            }
        }
    }
}