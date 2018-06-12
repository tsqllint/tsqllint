using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.UseCases.Console.HandlerStrategies
{
    public class PrintVersionStrategy : IHandlingStrategy
    {
        private readonly IBaseReporter reporter;

        public PrintVersionStrategy(IBaseReporter reporter)
        {
            this.reporter = reporter;
        }

        public HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            reporter.Report($"v{version}");
            return new HandlerResponseMessage(true, false);
        }
    }
}
