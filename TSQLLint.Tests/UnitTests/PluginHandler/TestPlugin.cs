using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;
using TSQLLint.Lib.Rules.RuleViolations;

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
                if (column > -1)
                {
                    reporter.ReportViolation(new RuleViolation(
                        context.FilePath,   
                        "prefer-tabs", 
                        "Should use spaces rather than tabs", 
                        lineNumber,
                        column,
                        RuleViolationSeverity.Warning));
                }
            }
        }
    }
}
