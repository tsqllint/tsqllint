using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TSQLLINT_LIB;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_TESTS.Unit_Tests.Rules
{
    public class SetTransactionIsolationLevelRuleTest
    {

        [TestCase(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT * FROM FOO;", 
                    0, 
                    Description = "Valid use of Transaction Isolation Level")]

        [TestCase(@"SELECT * FROM FOO;", 
                    1, 
                    Description = "Missing Transaction Isolation Level")]

        [TestCase(@"SELECT 1;
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;", 
                    1, 
                    Description = "Transaction Isolation Level Should Appear in one of the top 5 lines of the script")]

        [TestCase(@"SET ANSI_NULLS ON
                    SET QUOTED_IDENTIFIER ON
                    SET NOCOUNT ON
                    GO
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT * FROM FOO;",
                    0,
                    Description = "Transaction Isolation Level Should Appear in one of the top 5 lines of the script")]

        public void StatementTermination(string sqlString, int violationCount)
        {
            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action<string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new SetTransactionIsolationLevelRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);
            fragmentVisitor.VisistRule(textReader, visitor);

            Assert.AreEqual(violationCount, ruleViolations.Count);
        }
    }
}
