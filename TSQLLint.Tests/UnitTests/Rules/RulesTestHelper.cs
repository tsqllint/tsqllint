using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Rules.RuleViolations;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.UnitTests.Rules
{
    public static class RulesTestHelper
    {
        private static readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        public static void RunRulesTest(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            // arrange
            var sqlString = TestHelper.GetUnitTestFile(string.Format(@"Rules\{0}\test-files\{1}.sql", rule, testFileName));

            var fragmentVisitor = new SqlRuleVisitor(null, null);
            var ruleViolations = new List<RuleViolation>();

            Action<string, string, int, int> ErrorCallback = delegate(string ruleName, string ruleText, int startLine, int startColumn)
            {
                ruleViolations.Add(new RuleViolation(ruleName, startLine, startColumn));
            };

            var visitor = (TSqlFragmentVisitor)Activator.CreateInstance(ruleType, ErrorCallback);
            var textReader = TSQLLint.Lib.Utility.Utility.CreateTextReaderFromString(sqlString);

            var compareer = new RuleViolationCompare();

            // act
            fragmentVisitor.VisitRule(textReader, visitor);

            ruleViolations = ruleViolations.OrderBy(o => o.Line).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            // assert
            Assert.AreEqual(expectedRuleViolations.Count, ruleViolations.Count);
            CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, compareer);
        }
    }
}
