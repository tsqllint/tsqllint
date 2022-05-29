using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration.Overrides;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.UnitTests.LintingRuleExceptions
{
    [TestFixture]
    public class RuleExceptionFinderOverrideFinderIntegrationTests
    {
        private static readonly RuleExceptionFinder RuleExceptionFinder = new RuleExceptionFinder(RuleVisitorFriendlyNameTypeMap.List);
        private static readonly RuleExceptionComparer RuleExceptionComparer = new RuleExceptionComparer();

        private static readonly OverrideFinder OverrideFinder = new OverrideFinder();
        private static readonly OverrideComparer OverrideComparer = new OverrideComparer();

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "valid rule disable / enable",
                @"/* tsqllint-disable select-star */
                        SELECT * FROM FOO:
                  */ tsqllint-enable: select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), "select-star", 1, 3) },
                new List<IOverride>()
            },
            new object[]
            {
                "valid compatability-level override",
                @"/*
                    tsqllint-override compatability-level = 100
                    tsqllint-disable select-star
                  */
                        SELECT * FROM FOO:
                  */ tsqllint-enable select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), "select-star", 3, 6) },
                new List<IOverride> { new OverrideCompatabilityLevel("100") }
            },
            new object[]
            {
                "valid compatability-level override, ignore multiple rules",
                @"/*
                    tsqllint-disable select-star set-ansi
                    tsqllint-override compatability-level = 110
                  */
                        SELECT * FROM FOO:
                  */ tsqllint-enable: select-star */",
                new List<IExtendedRuleException> { new RuleException(typeof(SelectStarRule), "select-star", 2, 6), new RuleException(typeof(SetAnsiNullsRule), "set-ansi", 2, 6) },
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
