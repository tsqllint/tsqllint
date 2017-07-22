using System;
using System.Diagnostics;
using System.IO;
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
            var commandLineOptions  = new CommandLineParser.CommandLineParser(args, reporter);

            if (commandLineOptions != null && commandLineOptions.Init)
            {
                var configFileGenerator = new ConfigFileGenerator(reporter);

                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var configFilePath = Path.Combine(usersDirectory, @".tsqllintrc");

                configFileGenerator.WriteConfigFile(configFilePath);
                return;
            }

            if (commandLineOptions.Version)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var version = fvi.FileVersion;
                reporter.Report(string.Format("v{0}", version));
            }

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