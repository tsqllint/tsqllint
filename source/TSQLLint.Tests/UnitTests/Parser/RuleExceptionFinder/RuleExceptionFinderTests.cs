using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.UnitTests.Parser.RuleExceptionFinder
{
    [TestFixture]
    public class RuleExceptionFinderTests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "disable-rule-enable-another", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), "select-star", 1, 4),
                    new RuleException(typeof(SetAnsiNullsRule), "set-ansi", 1, 4)
                }
            },
            new object[]
            {
                "disable-rule-enable-rule", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), "select-star", 1, 5),
                    new RuleException(typeof(SetAnsiNullsRule), "set-ansi", 1, 5)
                }
            },
            new object[]
            {
                "disable-rule-whole-file", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), "select-star", 1, 9),
                    new RuleException(typeof(SemicolonTerminationRule), "semicolon-termination", 1, 9),
                    new RuleException(typeof(PrintStatementRule), "print-statement", 5, 9)
                }
            },
            new object[]
            {
                "globally-disable-multi-set", new List<IRuleException>()
                {
                    new GlobalRuleException(1, 5),
                    new GlobalRuleException(7, 9)
                }
            },
            new object[]
            {
                "globally-disable", new List<IRuleException>
                {
                    new GlobalRuleException(1, 3)
                }
            },
            new object[]
            {
                "globally-disable-re-enable", new List<IRuleException>
                {
                    new GlobalRuleException(1, 5),
                    new RuleException(typeof(SelectStarRule), "select-star", 7, 9),
                }
            },
            new object[]
            {
                "disable-rule-with-additional-comments", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), "select-star", 1, 3),
                    new RuleException(typeof(LinkedServerRule), "linked-server", 1, 3)
                }
            },
            new object[]
            {
                "global-disable-rule-with-additional-comments", new List<IRuleException>
                {
                    new GlobalRuleException(1, 3)
                }
            }
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void RuleExceptionFinderUnitTests(string testFileName, List<IRuleException> expectedResult)
        {
            // arrange
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"UnitTests/Parser/RuleExceptionFinder/TestFiles/{testFileName}.sql"));
            var fileStream = File.OpenRead(path);

            var ruleExceptionFinder = new Infrastructure.Rules.RuleExceptions.RuleExceptionFinder(RuleVisitorFriendlyNameTypeMap.List);
            var comparer = new RuleExceptionComparer();

            // act
            var result = ruleExceptionFinder.GetIgnoredRuleList(fileStream);

            // assert
            expectedResult = expectedResult.OrderByDescending(x => x.StartLine).ToList();
            result = result.OrderByDescending(x => x.StartLine).ToList();
            CollectionAssert.AreEqual(expectedResult, result, comparer);
        }
    }
}
