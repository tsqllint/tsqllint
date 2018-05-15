using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules;
using TSQLLint.Infrastructure.Rules.RuleViolations;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class DynamicSqlErrorPositionTest
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                @"EXEC('SELECT 1');",
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 1, 15),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1');",
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 1, 18),
                }
            },
            new object[]
            {
                @"EXECUTE('SELECT 1')", // inner and outer statements missing semicolon
                new List<RuleViolation>
                {
                    new RuleViolation("semicolon-termination", 1, 18),
                    new RuleViolation("semicolon-termination", 1, 20),
                }
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void VisitRules_Dynamic_SQL(string sql, List<RuleViolation> expectedRuleViolations)
        {
            // arrange
            var ruleViolations = new List<RuleViolation>();
            var mockReporter = Substitute.For<IReporter>();
            var mockPath = string.Empty;
            var compareer = new RuleViolationComparer();
            var fragmentBuilder = new FragmentBuilder();
            var sqlStream = ParsingUtility.GenerateStreamFromString(sql);

            void ErrorCallback(string ruleName, string ruleText, int startLine, int startColumn)
            {
                ruleViolations.Add(new RuleViolation(ruleName, startLine, startColumn));
            }

            var visitors = new List<TSqlFragmentVisitor>
            {
                new SemicolonTerminationRule(ErrorCallback)
            };

            var mockRuleVisitorBuilder = Substitute.For<IRuleVisitorBuilder>();
            mockRuleVisitorBuilder.BuildVisitors(Arg.Any<string>(), Arg.Any<IEnumerable<IRuleException>>()).Returns(visitors);

            var sqlRuleVisitor = new SqlRuleVisitor(mockRuleVisitorBuilder, fragmentBuilder, mockReporter);

            // act
            sqlRuleVisitor.VisitRules(mockPath, new List<IRuleException>(), sqlStream);

            ruleViolations = ruleViolations.OrderBy(o => o.Line).ThenBy(o => o.Column).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ThenBy(o => o.Column).ToList();

            // assert
            CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, compareer);
        }
    }
}
