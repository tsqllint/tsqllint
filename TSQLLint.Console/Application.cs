using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Core.UseCases.Console;
using TSQLLint.Infrastructure;
using TSQLLint.Infrastructure.CommandLineOptions;
using TSQLLint.Infrastructure.Config;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.Reporters;

namespace TSQLLint.Console
{
    public class Application
    {
        private readonly ICommandLineOptionHandler commandLineOptionHandler;
        private readonly ICommandLineOptions commandLineOptions;
        private readonly IConfigReader configReader;
        private readonly IReporter reporter;
        private readonly IConsoleTimer timer;

        private IPluginHandler pluginHandler;
        private ISqlFileProcessor fileProcessor;

        public Application(string[] args, IReporter reporter)
        {
            timer = new ConsoleTimer();
            timer.Start();

            this.reporter = reporter;
            commandLineOptions = new CommandLineOptions(args);
            configReader = new ConfigReader(reporter);
            commandLineOptionHandler = new CommandLineOptionHandler(
                new ConfigFileGenerator(),
                configReader,
                reporter,
                new FileSystemWrapper());
        }

        public void Run()
        {
            configReader.LoadConfig(commandLineOptions.ConfigFile);

            var fragmentBuilder = new FragmentBuilder(configReader.CompatabilityLevel);
            var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, this.reporter);
            var ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
            pluginHandler = new PluginHandler(reporter);
            fileProcessor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, new FileSystem());

            pluginHandler.ProcessPaths(configReader.GetPlugins());
            commandLineOptionHandler.HandleCommandLineOptions(commandLineOptions);
            fileProcessor.ProcessList(commandLineOptions.LintPath);

            if (fileProcessor.FileCount > 0)
            {
                reporter.ReportResults(timer.Stop(), fileProcessor.FileCount);
            }
        }
    }
}
