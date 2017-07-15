using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Integration_Tests.ExceptionalCases
{
    public class IntegrationExceptionalCases {

        [Test]
        public void RuleValidationTest()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\ExceptionalCases\\invalid-syntax");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IResultReporter testReporter = new IntegrationExceptionalCaseTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor);

            var lintFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\ExceptionalCases\\invalid-syntax\\invalid-syntax.sql");
            fileProcessor.ProcessPath(lintFile);
            testReporter.ReportResults(ruleVisitor.Violations);
        }
    }

    public class IntegrationExceptionalCaseTestReporter: IResultReporter
    {
        public void ReportResults(List<RuleViolation> violations)
        {
            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual("TSQL not syntactically correct", violations[0].Text);
        }
    }
}