using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.RuleVisitorBuilder
{
    public class RuleVisitorBuilderTest
    {
        [Test]
        public void GetRuleSeverity()
        {
            var configFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\RuleVisitorBuilder\\.tsqllintrc");
            var configReader = new ConfigReader();
            configReader.LoadConfigFromFile(configFilePath);
            var ruleVisitorBuilder = new TSQLLINT_LIB.Parser.RuleVisitorBuilder(configReader, null);

            var violations = new List <RuleViolation> ();
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors("foo", violations);

            Assert.AreEqual(2, activeRuleVisitors.Count);
        }
    }
}