using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;

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
            var RuleVisitorBuilder = new Lib.Parser.RuleVisitorBuilder(ConfigReader, null);
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors("foo");

            Assert.AreEqual(2, ActiveRuleVisitors.Count);
        }
    }
}
