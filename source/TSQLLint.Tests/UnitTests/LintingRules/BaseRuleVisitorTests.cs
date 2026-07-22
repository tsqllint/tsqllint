using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules.Common;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.LintingRules
{
    [TestFixture]
    public class BaseRuleVisitorTests
    {
        [Test]
        public void GetColumnNumber_ForToken_AddsDynamicSqlOffsetOnFirstLine()
        {
            var rule = new TestableRule
            {
                DynamicSqlStartLine = 1,
                DynamicSqlStartColumn = 10
            };

            var token = GetFirstRealToken("SELECT 1;");

            // token is on line 1, so the dynamic-sql column offset (10) is applied
            Assert.That(rule.PublicGetColumnNumber(token), Is.EqualTo(token.Column + 10));
        }

        [Test]
        public void GetColumnNumber_ForToken_NoOffsetWhenNotDynamicSql()
        {
            var rule = new TestableRule();

            var token = GetFirstRealToken("SELECT 1;");

            Assert.That(rule.PublicGetColumnNumber(token), Is.EqualTo(token.Column));
        }

        [Test]
        public void FixViolation_BaseImplementation_IsNoOpAndDoesNotThrow()
        {
            var rule = new TestableRule();
            var fileLines = new List<string> { "SELECT 1;" };
            var violations = new List<IRuleViolation>
            {
                new RuleViolation("file.sql", "testable", 1, 1)
            };
            var actions = new FileLineActions(violations, fileLines);

            Assert.That(() => rule.FixViolation(fileLines, violations[0], actions), Throws.Nothing);

            // the base implementation must leave the file untouched
            Assert.That(fileLines, Is.EqualTo(new List<string> { "SELECT 1;" }));
        }

        private static TSqlParserToken GetFirstRealToken(string sql)
        {
            using var reader = new StringReader(sql);
            var fragment = new FragmentBuilder(120).GetFragment(reader, out _);

            foreach (var token in fragment.ScriptTokenStream)
            {
                if (token.TokenType != TSqlTokenType.WhiteSpace)
                {
                    return token;
                }
            }

            throw new InvalidOperationException("no token found");
        }

        private class TestableRule : BaseRuleVisitor
        {
            public TestableRule()
                : base((_, _, _, _) => { })
            {
            }

            public override string RULE_NAME => "testable";

            public override string RULE_TEXT => "testable rule";

            public int PublicGetColumnNumber(TSqlParserToken token) => GetColumnNumber(token);
        }
    }
}
