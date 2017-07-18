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

            var commandLineHelper  = new CommandLineParser.CommandLineParser(args);
            var commandLineOptions = commandLineHelper.GetCommandLineOptions();

            if (commandLineOptions == null)
            {
                return;
            }

            if (commandLineOptions.Init)
            {
                // write config file and exit
                ConfigFileGenerator.WriteConfigFile();
                return;
            }

            var configReader = new LintConfigReader(commandLineOptions.ConfigFile);
            var ruleVisitor = new SqlRuleVisitor(configReader);
            var parser = new SqlFileProcessor(ruleVisitor);
            parser.ProcessPath(commandLineOptions.LintPath);
            var reporter = new ConsoleReporter();

            stopWatch.Stop();
            var timespan = stopWatch.Elapsed;

            reporter.ReportResults(ruleVisitor.Violations, timespan, parser.GetFileCount());
        }
    }
}