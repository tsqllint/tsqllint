using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.LintingRuleVisitorBuilder
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);
            var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, null);

            var ignoredRuleList = new List<IExtendedRuleException>();
            var activeRuleVisitors = ruleVisitorBuilder.BuildVisitors("foo.sql", ignoredRuleList);
            Assert.AreEqual(2, activeRuleVisitors.Count);
        }
    }
}
