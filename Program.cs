using System;
using System.IO;
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

            var configFile = File.ReadAllText(commandLineOptions.ConfigFile);
           
            var configReader = new LintConfigReader(configFile);
            var ruleVisitor = new SqlRuleVisitor();
            var reporter = new ConsoleResultReporter();
            var parser = new SqlFileProcessor(ruleVisitor, configReader);

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

