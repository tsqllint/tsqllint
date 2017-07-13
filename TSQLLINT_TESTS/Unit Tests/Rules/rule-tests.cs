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
    public class SelectStarRuleTests
    {
        private readonly TestHelper TestHelper = new TestHelper(TestContext.CurrentContext.TestDirectory);

        [TestCase("data-compression", typeof(DataCompressionOptionRule), "data-compression-no-error", 0)]
        [TestCase("data-compression", typeof(DataCompressionOptionRule), "data-compression-one-error-mixed-state", 1)]
        [TestCase("data-compression", typeof(DataCompressionOptionRule), "data-compression-one-error", 1)]
        [TestCase("data-compression", typeof(DataCompressionOptionRule), "data-compression-two-errors", 2)]

        [TestCase("data-type-length", typeof(DataTypeLengthRule), "data-type-length-no-error", 0)]
        [TestCase("data-type-length", typeof(DataTypeLengthRule), "data-type-length-one-error-mixed-state", 1)]
        [TestCase("data-type-length", typeof(DataTypeLengthRule), "data-type-length-one-error", 1)] 
        [TestCase("data-type-length", typeof(DataTypeLengthRule), "data-type-length-two-errors", 2)]
        [TestCase("data-type-length", typeof(DataTypeLengthRule), "data-type-length-all-errors", 11)]

        [TestCase("information-schema", typeof(InformationSchemaRule), "information-schema-no-error", 0)]
        [TestCase("information-schema", typeof(InformationSchemaRule), "information-schema-one-error-mixed-state", 1)]
        [TestCase("information-schema", typeof(InformationSchemaRule), "information-schema-one-error", 1)]
        [TestCase("information-schema", typeof(InformationSchemaRule), "information-schema-two-errors", 2)]

        [TestCase("object-property", typeof(ObjectPropertyRule), "object-property-no-error", 0)]
        [TestCase("object-property", typeof(ObjectPropertyRule), "object-property-one-error-mixed-state", 1)]
        [TestCase("object-property", typeof(ObjectPropertyRule), "object-property-one-error", 1)]
        [TestCase("object-property", typeof(ObjectPropertyRule), "object-property-two-errors", 2)]

        [TestCase("schema-qualify", typeof(SchemaQualifyRule), "schema-qualify-no-error", 0)]
        [TestCase("schema-qualify", typeof(SchemaQualifyRule), "schema-qualify-one-error-mixed-state", 1)]
        [TestCase("schema-qualify", typeof(SchemaQualifyRule), "schema-qualify-one-error", 1)]
        [TestCase("schema-qualify", typeof(SchemaQualifyRule), "schema-qualify-two-errors", 2)]

        [TestCase("set-ansi", typeof(SetAnsiNullsRule), "set-ansi-no-error", 0)]
        [TestCase("set-ansi", typeof(SetAnsiNullsRule), "set-ansi-one-error-mixed-state", 1)]
        [TestCase("set-ansi", typeof(SetAnsiNullsRule), "set-ansi-one-error", 1)]

        [TestCase("select-star", typeof(SelectStarRule), "select-star-no-error", 0)]
        [TestCase("select-star", typeof(SelectStarRule), "select-star-one-error-mixed-state", 1)]
        [TestCase("select-star", typeof(SelectStarRule), "select-star-one-error", 1)]
        [TestCase("select-star", typeof(SelectStarRule), "select-star-two-errors", 2)]

        [TestCase("set-nocount", typeof(SetNoCountRule), "set-nocount-no-error", 0)]
        [TestCase("set-nocount", typeof(SetNoCountRule), "set-nocount-one-error-mixed-state", 1)]
        [TestCase("set-nocount", typeof(SetNoCountRule), "set-nocount-one-error", 1)]

        [TestCase("set-quoted", typeof(SetQuotedIdentifierRule), "set-quoted-no-error", 0)]
        [TestCase("set-quoted", typeof(SetQuotedIdentifierRule), "set-quoted-one-error-mixed-state", 1)]
        [TestCase("set-quoted", typeof(SetQuotedIdentifierRule), "set-quoted-one-error", 1)]

        [TestCase("statement-semicolon-termination", typeof(SemicolonRule), "statement-semicolon-termination-no-error", 0)]
        [TestCase("statement-semicolon-termination", typeof(SemicolonRule), "statement-semicolon-termination-one-error-mixed-state", 1)]
        [TestCase("statement-semicolon-termination", typeof(SemicolonRule), "statement-semicolon-termination-one-error", 1)]
        [TestCase("statement-semicolon-termination", typeof(SemicolonRule), "statement-semicolon-termination-two-errors", 2)]

        [TestCase("set-transaction-isolation-level", typeof(SetTransactionIsolationLevelRule), "set-transaction-isolation-level-no-error", 0)]
        [TestCase("set-transaction-isolation-level", typeof(SetTransactionIsolationLevelRule), "set-transaction-isolation-level-one-error-mixed-state", 1)]
        [TestCase("set-transaction-isolation-level", typeof(SetTransactionIsolationLevelRule), "set-transaction-isolation-level-one-error", 1)]

        [TestCase("upper-lower", typeof(UpperLowerRule), "upper-lower-no-error", 0)]
        [TestCase("upper-lower", typeof(UpperLowerRule), "upper-lower-one-error-mixed-state", 1)]
        [TestCase("upper-lower", typeof(UpperLowerRule), "upper-lower-one-error", 1)]
        [TestCase("upper-lower", typeof(UpperLowerRule), "upper-lower-two-errors", 2)]
  
        public void RuleTests(string rule, Type ruleType, string testFileName, int violationCount)
        {
            // arrange
            var sqlString = TestHelper.GetTestFile(string.Format("Rules\\{0}\\{1}.sql", rule, testFileName));

            var fragmentVisitor = new SqlRuleVisitor(null);
            var ruleViolations = new List<RuleViolation>();

            Action <string, string, TSqlFragment> ErrorCallback = delegate (string ruleName, string ruleText, TSqlFragment node)
            {
                ruleViolations.Add(new RuleViolation("SOME_PATH", ruleName, ruleText, node, RuleViolationSeverity.Error));
            };

            var visitor = (TSqlFragmentVisitor)Activator.CreateInstance(ruleType, ErrorCallback);
            var textReader = Utility.CreateTextReaderFromString(sqlString);

            // act
            fragmentVisitor.VisistRule(textReader, visitor);

            // assert
            Assert.AreEqual(violationCount, ruleViolations.Count);

            foreach (var ruleViolation in ruleViolations)
            {
                Assert.AreEqual(ruleViolation.RuleName, rule);
            }
        }
    }
}
