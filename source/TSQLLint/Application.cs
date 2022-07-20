using System;
using System.Collections.Generic;
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

            if (response.ShouldLint)
            {
                int? firstViolitionCount = null;
                List<IRuleViolation> violitions = null;
                List<IRuleViolation> previousViolations = null;
                const int maxPasses = 10;
                var passCount = 0;

                do
                {
                    var fragmentBuilder = new FragmentBuilder(configReader.CompatabilityLevel);
                    var rules = RuleVisitorFriendlyNameTypeMap.Rules;
                    var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, this.reporter, rules);
                    var ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
                    pluginHandler = new PluginHandler(reporter, rules);
                    pluginHandler.ProcessPaths(configReader.GetPlugins());
                    fileProcessor = new SqlFileProcessor(
                        ruleVisitor, pluginHandler, reporter, new FileSystem(), rules.ToDictionary(x => x.Key, x => x.Value.GetType()));

                    passCount++;
                    previousViolations = violitions;

                    reporter.ShouldCollectViolations = response.ShouldFix;
                    reporter.ClearViolations();
                    fileProcessor.ProcessList(commandLineOptions.LintPath);

                    // Prevent the reportor from douple or tripple counting errors if the while loop evaulates to true;
                    reporter.ReporterMuted = true;

                    if (response.ShouldFix)
                    {
                        new ViolationFixer(new FileSystem(), rules, reporter.Violations).Fix();

                        violitions = reporter.Violations;

                        if (!firstViolitionCount.HasValue)
                        {
                            firstViolitionCount = violitions.Count;
                        }
                    }
                }
                while (response.ShouldFix && violitions.Count > 0 && !AreEqual(violitions, previousViolations) && passCount < maxPasses);

                if (fileProcessor.FileCount > 0)
                {
                    reporter.FixedCount = firstViolitionCount - violitions?.Count;
                    reporter.ReportResults(timer.Stop(), fileProcessor.FileCount);
                }
            }

            if (!response.Success)
            {
                Environment.ExitCode = 1;
            }
        }

        private bool AreEqual(List<IRuleViolation> violitions, List<IRuleViolation> previousViolations)
        {
            return violitions.All(x => previousViolations?.Any(y
                => x.RuleName == y.RuleName && x.Line == y.Line && x.Column == y.Column) == true) &&
                previousViolations?.All(x => violitions.Any(y
                => x.RuleName == y.RuleName && x.Line == y.Line && x.Column == y.Column)) == true;
        }
    }
}
