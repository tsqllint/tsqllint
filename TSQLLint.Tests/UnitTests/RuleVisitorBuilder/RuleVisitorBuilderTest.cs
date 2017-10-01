using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.RuleVisitorBuilder
{
    public class RuleVisitorBuilderTest
    {
        [Test]
        public void GetRuleSeverity()
        {
            var reporter = Substitute.For<IReporter>();
            var configfilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..\\UnitTests\\RuleVisitorBuilder\\.tsqllintrc");
            var ConfigReader = new ConfigReader(reporter);
            ConfigReader.LoadConfigFromFile(configfilePath);
            var RuleVisitorBuilder = new TSQLLint.Lib.Parser.RuleVisitorBuilder(ConfigReader, null);

            var violations = new List<RuleViolation>();
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors("foo", violations);

            Assert.AreEqual(2, ActiveRuleVisitors.Count);
        }
    }
}
