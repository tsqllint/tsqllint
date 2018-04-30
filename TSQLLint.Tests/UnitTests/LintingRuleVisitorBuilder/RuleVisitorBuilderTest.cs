using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Config;

namespace TSQLLint.Tests.UnitTests.LintingRuleVisitorBuilder
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);
            var ruleVisitorBuilder = new Infrastructure.RuleVisitorBuilder(configReader, null);

            var ignoredRuleList = new List<IExtendedRuleException>();
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors("foo.sql", ignoredRuleList);
            Assert.AreEqual(2, activeRuleVisitors.Count);
        }
    }
}
