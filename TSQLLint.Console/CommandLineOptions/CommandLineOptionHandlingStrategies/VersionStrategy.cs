using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;

namespace TSQLLint.Console.CommandLineOptions.CommandLineOptionHandlingStrategies
{
    public class VersionStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter _reporter;

        public VersionStrategy(IBaseReporter reporter)
        {
            _reporter = reporter;
        }
        
        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            _reporter.Report($"v{version}");
        }
    }
}
