using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Console.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionStrategies
{
    public class PrintVersionStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;

        public PrintVersionStrategy(IBaseReporter reporter)
        {
            _reporter = reporter;
        }
        
        public void HandleCommandLineOptions(ICommandLineOptions commandLineOptions = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            _reporter.Report($"v{version}");
        }
    }
}
