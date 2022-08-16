using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    [ExcludeFromCodeCoverage]
    public class TestPlugin : IPlugin
    {
        public void PerformAction(IPluginContext context, IReporter reporter)
        {
            string line;
            var lineNumber = 0;

            while ((line = context.FileContents.ReadLine()) != null)
            {
                lineNumber++;
                var column = line.IndexOf("\t", StringComparison.Ordinal);

                if (column >= 0)
                {
                    reporter.ReportViolation(new TestRuleViolation(
                        context.FilePath,
                        "prefer-tabs",
                        "Should use spaces rather than tabs",
                        lineNumber,
                        column,
                        RuleViolationSeverity.Warning));
                }
            }
        }

        public IDictionary<string, ISqlLintRule> GetRules() => new Dictionary<string, ISqlLintRule>
        {
            ["plugin-rule"] = new TestPluginRule(null)
        };
    }
}
