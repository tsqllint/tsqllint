using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\IntegrationTests\\ExceptionalCases\\invalid-syntax");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            IReporter testReporter = new IntegrationExceptionalCaseTestReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, testReporter);

            var lintFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\IntegrationTests\\ExceptionalCases\\invalid-syntax\\invalid-syntax.sql");
            fileProcessor.ProcessPath(lintFile);
            testReporter.ReportResults(ruleVisitor.Violations, new TimeSpan(), 0);

            Assert.Throws<NotImplementedException>(() => { testReporter.Report("foo"); });
        }
    }

    public class IntegrationExceptionalCaseTestReporter: IReporter
    {
        public void ReportResults(List<RuleViolation> violations, TimeSpan timespan, int fileCount)
        {
            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual("TSQL not syntactically correct", violations[0].Text);
        }

        public void Report(string message)
        {
            throw new NotImplementedException();
        }
    }
}