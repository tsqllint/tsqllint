using System;
using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Integration_Tests.ExceptionalCases
{
    public class IntegrationExceptionalCases {

        [Test]
        public void RuleValidationTest()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\ExceptionalCases\\invalid-syntax");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, configReader);

            Assert.Throws<Exception>(() => fileProcessor.ProcessPath(lintTarget));
        }
    }
}