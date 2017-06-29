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
    public class SetTransactionIsolationLevelTests
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [TestCase("set-transaction-isolation-level-no-error",              0, Description = "one statement, one set-transaction-isolation-level rule violation")]
        [TestCase("set-transaction-isolation-level-one-error-mixed-state", 1, Description = "two statements, one set-transaction-isolation-level rule violation")]
        [TestCase("set-transaction-isolation-level-one-error",             1, Description = "one statement, one set-transaction-isolation-level rule violation")]
        public void SelectStarRuleVisitorTests(string testFileName, int violationCount)
        {
            // arrange
            var sqlString = TestHelper.GetTestFile(string.Format("Rules\\set-transaction-isolation-level\\test files\\{0}.sql", testFileName));

            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action <string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new SetTransactionIsolationLevelRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);

            // act
            fragmentVisitor.VisistRule(textReader, visitor);

            // assert
            Assert.AreEqual(violationCount, ruleViolations.Count);

            foreach (var ruleViolation in ruleViolations)
            {
                Assert.AreEqual(ruleViolation.RuleName, "set-transaction-isolation-level");
                Assert.AreEqual(ruleViolation.Text, "Set Transaction Isolation Level Read Uncommitted Should Appear Before Other Statements");
            }
        }
    }
}
