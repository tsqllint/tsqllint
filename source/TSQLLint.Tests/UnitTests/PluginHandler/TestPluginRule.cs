using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    [ExcludeFromCodeCoverage]
    public class TestPluginRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public TestPluginRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "no-comments";

        public string RULE_TEXT => "Should not have comments";

        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Error;

        public override void Visit(TSqlScript node)
        {
            foreach (var token in node.ScriptTokenStream)
            {
                if (token.TokenType != TSqlTokenType.SingleLineComment)
                {
                    continue;
                }

                errorCallback(RULE_NAME, RULE_TEXT, token.Line, token.Column);
                break;
            }
        }

        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
        }
    }
}
