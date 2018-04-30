using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLint.Infrastructure;
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

            var visitor = (TSqlFragmentVisitor)Activator.CreateInstance(ruleType, args: (Action<string, string, int, int>)ErrorCallback);
            var compareer = new RuleViolationComparer();

            var fragmentBuilder = new FragmentBuilder(120);
            var textReader = new StreamReader(fileStream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out _);

            // act
            sqlFragment.Accept(visitor);

            ruleViolations = ruleViolations.OrderBy(o => o.Line).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            // assert
            CollectionAssert.AreEqual(expectedRuleViolations, ruleViolations, compareer);
            Assert.AreEqual(expectedRuleViolations.Count, ruleViolations.Count);
        }
    }
}
