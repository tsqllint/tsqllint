using System;
using System.Diagnostics;
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

            var commandLineOptions = GetCommandLineOptions(args);
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
            var reporter = new ConsoleResultReporter();

            stopWatch.Stop();
            var timespan = stopWatch.Elapsed;

            reporter.ReportResults(ruleVisitor.Violations, timespan, parser.GetFileCount());
        }

        private static CommandLineOptions GetCommandLineOptions(string[] args)
        {
            var commandLineOptions = new CommandLineOptions();

            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, commandLineOptions))
            {
                IValidator<CommandLineOptions> optionsValidator = new OptionsValidator();
                var optionsValid = optionsValidator.Validate(commandLineOptions);

                if (!optionsValid)
                {
                    Console.WriteLine(commandLineOptions.GetUsage());
                    return null;
                }
            }
            else
            {
                Console.WriteLine(commandLineOptions.GetUsage());
                return null;
            }

            return commandLineOptions;
        }
    }
}

