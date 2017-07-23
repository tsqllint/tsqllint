using System.Diagnostics;
using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE
{
    public class Application
    {
        private readonly string[] Args;
        private IReporter Reporter;

        public Application(string[] args, IReporter reporter)
        {
            Args = args;
            Reporter = reporter;
        }

        public void Run()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // parse options
            var commandLineOptions = new ConsoleCommandLineOptionParser(Args);

            // perform non-linting actions
            var configHandler = new ConfigHandler.ConfigHandler(commandLineOptions, Reporter);
            configHandler.HandleConfigs();

            if (!configHandler.PerformLinting)
            {
                return;
            }

            // lint
            var lintingHandler = new LintingHandler(commandLineOptions, Reporter);
            lintingHandler.Lint();

            stopWatch.Stop();
            var timespan = stopWatch.Elapsed;

            Reporter.ReportResults(lintingHandler.RuleViolations, timespan, lintingHandler.LintedFileCount);
        }
    }
}