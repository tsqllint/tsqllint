using System;
using System.Diagnostics;   
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE.CommandLineParser
{
    public class CommandLineParser
    {
        private readonly string[] Args;

        public CommandLineParser(string[] args)
        {
            Args = args;
        }

        [Option(shortName: 'c', 
            longName: "config", 
            Required = false, 
            HelpText = "Use configuration from this file or shareable config.", 
            DefaultValue = ".tsqllintrc")]
        public string ConfigFile { get; set; }

        [Option(shortName: 'p',
            longName: "path",
            Required = false,
            HelpText = "Target path for linting")]
        public string LintPath { get; set; }

        [Option(shortName: 'i',
            longName: "init",
            Required = false,
            HelpText = "generate .tsqllintrc file")]
        public bool Init { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            var help = new HelpText
            {
                Heading = new HeadingInfo("TSQLLINT", version),
                Copyright = new CopyrightInfo("Nathan Boyd & Doug Wilson", 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("License: MIT https://opensource.org/licenses/MIT");
            help.AddPreOptionsLine("Usage: TSQLLINT [options]");
            help.AddOptions(this);
            return help;
        }

        public CommandLineParser GetCommandLineOptions()
        {
            var commandLineOptions = new CommandLineParser(Args);

            if (Parser.Default.ParseArgumentsStrict(Args, commandLineOptions))
            {
                IValidator<CommandLineParser> optionsValidator = new OptionsValidator();
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