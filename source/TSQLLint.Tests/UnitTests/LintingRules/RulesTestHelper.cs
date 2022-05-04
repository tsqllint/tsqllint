using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules.Common;
using TSQLLint.Infrastructure.Rules.RuleViolations;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    public static class RulesTestHelper
    {
        public static void RunRulesTest(string rule, string testFileName, Type ruleType, List<RuleViolation> expectedRuleViolations)
        {
            // arrange
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"UnitTests/LintingRules/{rule}/test-files/{testFileName}.sql"));
            var fileStream = File.OpenRead(path);

            var ruleViolations = new List<RuleViolation>();

            void ErrorCallback(string ruleName, string ruleText, int startLine, int startColumn)
            {
                ruleViolations.Add(new RuleViolation(ruleName, startLine, startColumn));
            }

            var visitor = GetVisitor(ruleType, ErrorCallback);
            var comparer = new RuleViolationComparer();

            var fragmentBuilder = new FragmentBuilder(120);
            var textReader = new StreamReader(fileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);

            // act
            sqlFragment.Accept(visitor);

            ruleViolations = ruleViolations.OrderBy(o => o.Line).ThenBy(o => o.Column).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ThenBy(o => o.Column).ToList();

            // assert
            CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, comparer);
            Assert.AreEqual(expectedRuleViolations.Count, ruleViolations.Count);
        }

        public static void RunRulesTestWithFix(string rule, string testFileName, Type ruleType)
        {
            // arrange
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"UnitTests/LintingRules/{rule}/test-files/{testFileName}.sql"));
            var fixedPath = path.Replace(".sql", ".fixed.sql");
            var expectedPath = path.Replace(".sql", ".expected.sql");
            File.WriteAllText(fixedPath, File.ReadAllText(path));

            var ruleViolations = new List<IRuleViolation>();

            void ErrorCallback(string ruleName, string ruleText, int startLine, int startColumn)
            {
                var violation = new RuleViolation(fixedPath, ruleName, startLine, startColumn);
                ruleViolations.Add(violation);
            }

            var visitor = GetVisitor(ruleType, ErrorCallback);

            var fragmentBuilder = new FragmentBuilder(120);
            var fileStream = File.OpenRead(path);
            var textReader = new StreamReader(fileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);
            textReader.Close();

            // act
            sqlFragment.Accept(visitor);

            new ViolationFixer(new FileSystem(), ruleViolations).Fix();

            ruleViolations.Clear();
            fragmentBuilder = new FragmentBuilder(120);
            fileStream = File.OpenRead(fixedPath);
            textReader = new StreamReader(fileStream);
            sqlFragment = fragmentBuilder.GetFragment(textReader, out _);
            textReader.Close();

            // act
            sqlFragment.Accept(visitor);

            // assert
            Assert.Zero(ruleViolations.Count());

            if (File.Exists(expectedPath))
            {
                var expectedText = File.ReadAllText(expectedPath);
                var actualText = File.ReadAllText(fixedPath);
                Assert.AreEqual(expectedText, actualText);
            }
        }

        public static void RunDynamicSQLRulesTest(Type ruleType, string sql, List<RuleViolation> expectedRuleViolations)
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
                GetVisitor(ruleType, ErrorCallback)
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

        private static BaseRuleVisitor GetVisitor(Type ruleType, Action<string, string, int, int> errorCallback)
        {
            return (BaseRuleVisitor)Activator.CreateInstance(ruleType, errorCallback);
        }
    }
}
