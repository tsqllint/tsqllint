using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Tests.UnitTests.RuleVisitorBuilder
{
    [TestFixture]
    public class RuleVisitorBuilderTest
    {
        [Test]
        public void GetRuleSeverity()
        {
            var configFilePath = @"c:\.tsqllintrc";
            
            var jsonConfig = @"{
                'rules': {
                    'select-star': 'warning',
                    'semicolon-termination': 'error'
                }
            }";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {
                    configFilePath, new MockFileData(jsonConfig)
                }
            });

            var reporter = Substitute.For<IReporter>();
            var ConfigReader = new ConfigReader(reporter, fileSystem);
            ConfigReader.LoadConfigFromFile(configFilePath);
            var RuleVisitorBuilder = new Lib.Parser.RuleVisitorBuilder(ConfigReader, null);

            var ignoredRuleList = new List<IRuleException>();
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors("foo.sql", ignoredRuleList);
            Assert.AreEqual(2, ActiveRuleVisitors.Count);
        }
    }
}
