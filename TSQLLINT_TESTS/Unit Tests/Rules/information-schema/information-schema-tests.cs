using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLINT_LIB;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.Rules
{
    public class InformationSchemaTests
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [TestCase("information-schema-no-error",              0, Description = "one statement, one select-star rule violation")]
        [TestCase("information-schema-one-error-mixed-state", 1, Description = "two statements, one select-star rule violation")]
        [TestCase("information-schema-one-error",             1, Description = "one statement, one select-star rule violation")]
        [TestCase("information-schema-two-errors",            2, Description = "two statements, two select-star rule violations")]
        public void SelectStarRuleVisitorTests(string testFileName, int violationCount)
        {
            // arrange
            var sqlString = TestHelper.GetTestFile(string.Format("Rules\\information-schema\\test files\\{0}.sql", testFileName));

            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action <string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new InfirmationSchemaRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);

            // act
            fragmentVisitor.VisistRule(textReader, visitor);

            // assert
            Assert.AreEqual(violationCount, ruleViolations.Count);

            foreach (var ruleViolation in ruleViolations)
            {
                Assert.AreEqual(ruleViolation.RuleName, "information-schema");
                Assert.AreEqual(ruleViolation.Text, "Do not use the INFORMATION_SCHEMA views");
            }
        }
    }
}
