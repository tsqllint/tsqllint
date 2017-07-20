using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB_TESTS.Integration_Tests.ExceptionalCases
{
    public class InvalidLintTargetTests
    {
        [Test]
        public void InvalidLintPath()
        {
            var lintTarget = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Integration Tests\\ExceptionalCases\\invalid-lint-file-path\\");

            ILintConfigReader configReader = new LintConfigReader(Path.Combine(lintTarget, ".tsqllintrc"));
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(configReader);
            var reporter = new TestBaseReporter();
            var fileProcessor = new SqlFileProcessor(ruleVisitor, reporter);

            fileProcessor.ProcessPath("foo.sql");
            Assert.AreEqual(1, reporter.MessageCount);
        }

        private class TestBaseReporter : IBaseReporter
        {
            public int MessageCount;
            public void Report(string message)
            {
                MessageCount++;
                Assert.AreEqual("\n\nfoo.sql is not a valid path.", message);
            }
        }
    }
}