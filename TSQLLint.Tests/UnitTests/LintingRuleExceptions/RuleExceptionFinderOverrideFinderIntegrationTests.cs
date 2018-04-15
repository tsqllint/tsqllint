using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Parser.ConfigurationOverrides;
using TSQLLint.Lib.Parser.RuleExceptions;
using TSQLLint.Lib.Rules;
using TSQLLint.Lib.Utility;
using TSQLLint.Tests.Helpers.ObjectComparers;
using TSQLLint.Tests.UnitTests.ConfigFile.Overrides;

namespace TSQLLint.Tests.UnitTests.LintingRuleExceptions
{
    [TestFixture]
    public class RuleExceptionFinderOverrideFinderIntegrationTests
    {
        private static readonly RuleExceptionFinder RuleExceptionFinder = new RuleExceptionFinder();
        private static readonly RuleExceptionComparer RuleExceptionComparer = new RuleExceptionComparer();

        private static readonly OverrideFinder OverrideFinder = new OverrideFinder();
        private static readonly OverrideComparer OverrideComparer = new OverrideComparer();

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "valid rule disable / enable",
                @"/* tsqllint-disable: select-star */
                        SELECT * FROM FOO:
                  */ tsqllint-enable: select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), 1, 3) },
                new List<IOverride>()
            },
            new object[]
            {
                "valid compatability-level override",
                @"/*
                    tsqllint-override: compatability-level = 100
                    tsqllint-disable: select-star
                  */
                        SELECT * FROM FOO:
                  */ tsqllint-enable: select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), 3, 6) },
                new List<IOverride> { new OverrideCompatabilityLevel("100") }
            },
            new object[]
            {
                "valid compatability-level override, ignore multiple rules",
                @"/*
                    tsqllint-disable: select-star set-ansi
                    tsqllint-override: compatability-level = 110
                  */
                        SELECT * FROM FOO:
                  */ tsqllint-enable: select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), 2, 6), new RuleException(typeof(SetAnsiNullsRule), 2, 6) },
                new List<IOverride> { new OverrideCompatabilityLevel("110") }
            }
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestOverride(
            string message,
            string testString,
            List<IExtendedRuleException> expectedruleExceptionResult,
            List<IOverride> expectedOverrideResult)
        {
            var overrideResult = OverrideFinder.GetOverrideList(ParsingUtility.GenerateStreamFromString(testString));
            CollectionAssert.AreEqual(overrideResult, expectedOverrideResult, OverrideComparer, message);

            var ruleExceptionResult = RuleExceptionFinder.GetIgnoredRuleList(ParsingUtility.GenerateStreamFromString(testString));
            CollectionAssert.AreEqual(expectedruleExceptionResult, ruleExceptionResult, RuleExceptionComparer, message);
        }
    }
}
