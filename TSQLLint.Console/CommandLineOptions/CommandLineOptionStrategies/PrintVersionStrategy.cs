using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class PrintVersionStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;

        public PrintVersionStrategy(IBaseReporter reporter)
        {
            this.reporter = reporter;
        }

        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            reporter.Report($"v{version}");
        }
    }
}
