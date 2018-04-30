using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Core;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure;
using TSQLLint.Infrastructure.ConfigurationOverrides;

namespace TSQLLint.Tests.UnitTests.ConfigFile.Overrides
{
    [TestFixture]
    public class OverrideTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "valid compatability-level override",
                @"/* tsqllint-override: compatability-level = 130 */",
                new List<IOverride> { new OverrideCompatabilityLevel("130") }
            },
            new object[]
            {
                "valid compatability-level override and unsupported 'foo' override",
                @"/* tsqllint-override: compatability-level = 90, foo = bar */",
                new List<IOverride> { new OverrideCompatabilityLevel("90") }
            },
            new object[]
            {
                "valid compatability-level override within multiline comment block",
                @"/* 
                   tsqllint-disable: select-star
                   tsqllint-override: compatability-level = 80 
                */",
                new List<IOverride> { new OverrideCompatabilityLevel("80") }
            },
            new object[]
            {
                "valid compatability-level and unsupported override override within multiline comment block",
                @"/* 
                   tsqllint-disable: select-star
                   tsqllint-override: compatability-level = 80, foo = bar 
                */",
                new List<IOverride> { new OverrideCompatabilityLevel("80") }
            }
        };

        private readonly OverrideFinder testOverrideFinder = new OverrideFinder();
        private readonly OverrideComparer overrideComparer = new OverrideComparer();

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestOverride(string message, string testString, List<IOverride> expectedResult)
        {
            var overrides = testOverrideFinder.GetOverrideList(ParsingUtility.GenerateStreamFromString(testString));
            CollectionAssert.AreEqual(overrides, expectedResult, overrideComparer, message);
        }

        [Test]
        public void TestInvalidCompatabilityLevels()
        {
            var testOverrideCompatabilityLevel = new OverrideCompatabilityLevel("foo");
            Assert.AreEqual(Constants.DefaultCompatabilityLevel, testOverrideCompatabilityLevel.CompatabilityLevel);
        }
    }
}
