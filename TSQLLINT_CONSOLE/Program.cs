using System.Diagnostics;
using TSQLLINT_CONSOLE.CommandLineParser;
using TSQLLINT_CONSOLE.Reporters;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;

namespace TSQLLINT_CONSOLE
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var reporter = new ConsoleReporter();
            var commandLineOptions  = new ConsoleCommandLineOptionParser(args, reporter);
            var commandLineOptionHandler = new CommandLineOptionHandler();
            commandLineOptionHandler.HandleCommandLineOptions(commandLineOptions, reporter);

            if (!commandLineOptions.PerformLinting)
            {
                return;
            }

            var configReader = new LintConfigReader(commandLineOptions.ConfigFile);
            var ruleVisitor = new SqlRuleVisitor(configReader);
            var parser = new SqlFileProcessor(ruleVisitor, reporter);
            parser.ProcessPath(commandLineOptions.LintPath);

            stopWatch.Stop();
            var timespan = stopWatch.Elapsed;

            reporter.ReportResults(ruleVisitor.Violations, timespan, parser.GetFileCount());
        }

    }
}