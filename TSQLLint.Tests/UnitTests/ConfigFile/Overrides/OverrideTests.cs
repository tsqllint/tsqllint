using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.ConfigurationOverrides;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.UnitTests.ConfigFile.Overrides
{
    [TestFixture]
    public class OverrideTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                @"/* tsqllint-override: compatability-level = 130 */",
                new List<IOverride> { new OverrideCompatabilityLevel("130") }
            }
        };

        private readonly OverrideFinder testOverrideFinder = new OverrideFinder();
        private readonly OverrideComparer overrideComparer = new OverrideComparer();

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestOverride(string testString, List<IOverride> expectedResult)
        {
            var overrides = testOverrideFinder.GetOverrideList(ParsingUtility.GenerateStreamFromString(testString));
            CollectionAssert.AreEqual(overrides, expectedResult, overrideComparer);
        }
    }
}
