using System.Diagnostics;
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
            var commandLineHelper  = new CommandLineParser.CommandLineParser(args, reporter);
            var commandLineOptions = commandLineHelper.GetCommandLineOptions();

            if (commandLineOptions == null)
            {
                reporter.Report(commandLineHelper.GetUsage());
                return;
            }

            if (commandLineOptions.Init)
            {
                // write config file and exit
                ConfigFileGenerator.WriteConfigFile();
                return;
            }

            if (commandLineOptions.LintPath == null)
            {
                reporter.Report(commandLineHelper.GetUsage());
                return;
            }

            var configReader = new LintConfigReader(commandLineOptions.ConfigFile);
            var ruleVisitor = new SqlRuleVisitor(configReader);
            var parser = new SqlFileProcessor(ruleVisitor);
            parser.ProcessPath(commandLineOptions.LintPath);

            stopWatch.Stop();
            var timespan = stopWatch.Elapsed;

            reporter.ReportResults(ruleVisitor.Violations, timespan, parser.GetFileCount());
        }
    }
}