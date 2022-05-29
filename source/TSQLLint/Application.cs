using System;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;
using TSQLLint.Core.Interfaces.Config.Contracts;
using TSQLLint.Core.UseCases.Console;
using TSQLLint.Infrastructure.CommandLineOptions;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.Reporters;

namespace TSQLLint
{
    public class Application
    {
        private readonly IRequestHandler<CommandLineRequestMessage, HandlerResponseMessage> commandLineOptionHandler;
        private readonly ICommandLineOptions commandLineOptions;
        private readonly IConfigReader configReader;
        private readonly IConsoleReporter reporter;
        private readonly IConsoleTimer timer;

        private IPluginHandler pluginHandler;
        private ISqlFileProcessor fileProcessor;

        public Application(string[] args, IConsoleReporter reporter)
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

            var response = commandLineOptionHandler.Handle(new CommandLineRequestMessage(commandLineOptions));
            var fragmentBuilder = new FragmentBuilder(configReader.CompatabilityLevel);
            var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, this.reporter);
            var ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
            var rules = RuleVisitorFriendlyNameTypeMap.Rules;
            pluginHandler = new PluginHandler(reporter, rules);
            pluginHandler.ProcessPaths(configReader.GetPlugins());
            fileProcessor = new SqlFileProcessor(
                ruleVisitor, pluginHandler, reporter, new FileSystem(), rules.ToDictionary(x => x.Key, x => x.Value.GetType()));

            if (response.ShouldLint)
            {
                reporter.ShouldCollectViolations = response.ShouldFix;
                fileProcessor.ProcessList(commandLineOptions.LintPath);

                if (response.ShouldFix)
                {
                    new ViolationFixer(new FileSystem(), rules, reporter.Violations).Fix();
                }
            }

            if (fileProcessor.FileCount > 0)
            {
                reporter.ReportResults(timer.Stop(), fileProcessor.FileCount);
            }

            if (!response.Success)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
