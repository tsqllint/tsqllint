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
    public class SchemaQualifyTests
    {
        [TestCase(@"SELECT BAR FROM dbo.FOO;", 0)]
        [TestCase(@"SELECT BAR FROM FOO;", 1)]
        [TestCase(@"SELECT BAR FROM FOO
                    SELECT BAR FROM dbo.FOO;", 1)]
        public void SchemaQualifyTest(string sqlString, int violationCount)
        {
            var fragmentVisitor = new SqlRuleVisitor();
            var ruleViolations = new List<RuleViolation>();

            Action<string, string, TSqlFragment> ErrorCallback = delegate(string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = new SchemaQualifyRule(ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);
            fragmentVisitor.VisistRule(textReader, visitor);

            Assert.AreEqual(violationCount, ruleViolations.Count);
        }
    }
}
