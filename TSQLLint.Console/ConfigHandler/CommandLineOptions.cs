using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace TSQLLint.Console.ConfigHandler
{
    public class CommandLineOptions
    {
        public CommandLineOptions(string[] args)
        {
            Args = args;
            Parser.Default.ParseArgumentsStrict(args, this);
        }

        public string[] Args { get; set; }

        [Option('c',
             longName: "config",
             Required = false,
             HelpText = "Used to specify a .tsqllintrc file path other than the default")]
        public string ConfigFile { get; set; }

        public string DefaultConfigRules { get; set; }

        [Option('f',
            longName: "force",
            Required = false,
            HelpText = "Used to force generation of default config file when one already exists")]
        public bool Force { get; set; }

        [Option('i',
            longName: "init",
            Required = false,
            HelpText = "Generate default .tsqllintrc config file")]
        public bool Init { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> LintPath { get; set; }

        [Option('p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to config file"),
        TSQLLintOption(NonLintingCommand = true)]
        public bool PrintConfig { get; set; }

        [Option('v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version"),
        TSQLLintOption(NonLintingCommand = true)]
        public bool Version { get; set; }

        [Option('h',
            longName: "help",
            Required = false,
            HelpText = "Display this help dialog")]
        public bool Help { get; set; }

        [HelpVerbOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("tsqllint [options] [file.sql] | [dir] | [file.sql | dir]");
            help.AddOptions(this);
            return help;
        }
    }
}
