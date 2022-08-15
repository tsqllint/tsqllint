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

        public string RULE_NAME => "plugin-rule";

        public string RULE_TEXT => "Plugin rule failed";

        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Error;

        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
        }
    }
}
