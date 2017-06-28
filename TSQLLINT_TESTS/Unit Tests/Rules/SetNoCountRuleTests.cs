using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLINT_LIB;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_TESTS.Unit_Tests.Rules
{
    public class SetNoCountRuleTests
    {
        [TestCase(@"SET NOCOUNT ON
                    SELECT BAR FROM FOO;", 0)]
        [TestCase(@"SET ANSI_NULLS ON
                    SET QUOTED_IDENTIFIER ON
                    SET NOCOUNT ON
                    GO
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT * FROM FOO;", 0)]
        [TestCase(@"SELECT BAR FROM FOO;", 1)]
        public void SetNoCountRuleTest(string sqlString, int violationCount)
        {
            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action<string, string, TSqlFragment> ErrorCallback = delegate(string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new SetNoCountRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);
            fragmentVisitor.VisistRule(textReader, visitor);

            Assert.AreEqual(violationCount, ruleViolations.Count);
        }
    }
}