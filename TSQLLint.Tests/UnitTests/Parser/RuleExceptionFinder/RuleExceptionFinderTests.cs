using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Parser.RuleExceptions;
using TSQLLint.Lib.Rules;
using TSQLLint.Tests.Helpers;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.UnitTests.Parser.RuleExceptionFinder
{
    [TestFixture]
    public class RuleExceptionFinderTests
    {
        private static readonly object[] testCases =
        {
            new object[]
            {
                "disable-rule-enable-rule", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), 1, 5),
                    new RuleException(typeof(SetAnsiNullsRule), 1, 5)
                }
            },
            new object[]
            {
                "disable-rule-whole-file", new List<IRuleException>
                {
                    new RuleException(typeof(SelectStarRule), 1, 9),
                    new RuleException(typeof(SemicolonTerminationRule), 1, 9),
                    new RuleException(typeof(PrintStatementRule), 5, 9)
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
                    new RuleException(typeof(SelectStarRule), 7, 9),
                }
            }
        };

        [Test, TestCaseSource(nameof(testCases))]
        public void RuleExceptionFinderUnitTests(string testFileName, List<IRuleException> expectedResult)
        {
            // arrange
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"UnitTests/Parser/RuleExceptionFinder/TestFiles/{testFileName}.sql"));
            var fileStream = File.OpenRead(path);

            var ruleExceptionFinder = new Lib.Parser.RuleExceptions.RuleExceptionFinder();
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
