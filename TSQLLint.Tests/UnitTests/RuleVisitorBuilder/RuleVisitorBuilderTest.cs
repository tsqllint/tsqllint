using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [SetUp]
        [ExcludeFromCodeCoverage]
        public void Setup()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Assert.Ignore("Tests ignored on osx or linux until https://github.com/tathamoddie/System.IO.Abstractions/issues/252 is resolved");
            }
        }

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
            ConfigReader.LoadConfig(configFilePath);
            var RuleVisitorBuilder = new Lib.Parser.RuleVisitorBuilder(ConfigReader, null);

            var ignoredRuleList = new List<IRuleException>();
            var ActiveRuleVisitors = RuleVisitorBuilder.BuildVisitors("foo.sql", ignoredRuleList);
            Assert.AreEqual(2, ActiveRuleVisitors.Count);
        }
    }
}
