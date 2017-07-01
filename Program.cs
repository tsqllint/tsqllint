using System;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser;

namespace TSQLLINT
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            var commandLineOptions = GetCommandLineOptions(args);
            if (commandLineOptions == null)
            {
                return;
            }

            var configReader = new LintConfigReader(commandLineOptions.ConfigFile);
            var ruleVisitor = new SqlRuleVisitor(configReader);
            var reporter = new ConsoleResultReporter();
            var parser = new SqlFileProcessor(ruleVisitor);

            parser.ProcessPath(commandLineOptions.LintPath);
            reporter.ReportResults(ruleVisitor.Violations);
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

