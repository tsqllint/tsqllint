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
    public class StatementSemicolonTermination
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [TestCase("statement-semicolon-termination-no-error",              0, Description = "one statement, one statement-semicolon-termination rule violation")]
        [TestCase("statement-semicolon-termination-one-error-mixed-state", 1, Description = "two statements, one statement-semicolon-termination rule violation")]
        [TestCase("statement-semicolon-termination-one-error",             1, Description = "one statement, one statement-semicolon-termination rule violation")]
        [TestCase("statement-semicolon-termination-two-errors",            2, Description = "two statements, two statement-semicolon-termination rule violations")]
        public void SelectStarRuleVisitorTests(string testFileName, int violationCount)
        {
            // arrange
            var sqlString = TestHelper.GetTestFile(string.Format("Rules\\statement-semicolon-termination\\test files\\{0}.sql", testFileName));

            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action <string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new TerminateStatementsWithSemicolonRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);

            // act
            fragmentVisitor.VisistRule(textReader, visitor);

            // assert
            Assert.AreEqual(violationCount, ruleViolations.Count);

            foreach (var ruleViolation in ruleViolations)
            {
                Assert.AreEqual(ruleViolation.RuleName, "statement-semicolon-termination");
                Assert.AreEqual(ruleViolation.Text, "Terminate statements with semicolon");
            }
        }
    }
}
