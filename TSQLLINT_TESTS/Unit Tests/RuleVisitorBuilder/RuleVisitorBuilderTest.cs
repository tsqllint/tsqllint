using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.RuleVisitorBuilder
{
    class RuleVisitorBuilderTest
    {
        [Test]
        public void GetRuleSeverity()
        {
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\Unit Tests\\RuleVisitorBuilder\\.tsqllintrc");
            var ConfigReader = new LintConfigReader(configfilePath);
            var RuleVisitorBuilder = new TSQLLINT_LIB.Parser.RuleVisitorBuilder(ConfigReader);

            var violations = new List <RuleViolation> ();
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors("foo", violations);

            Assert.AreEqual(2, ActiveRuleVisitors.Count);
        }
    }
}
