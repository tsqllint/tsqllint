using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Common;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.PluginHandler
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
