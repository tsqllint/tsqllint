using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class CommandLineOptions
    {
        public CommandLineOptions(string[] args)
        {
            Args = args;
            Parser.Default.ParseArgumentsStrict(args, this);
        }

        public string[] Args { get; set; }

        [Option(shortName: 'c',
             longName: "config",
             Required = false,
             HelpText = "Used to specify a .tsqllintrc file path other than the default.")]
        public string ConfigFile { get; set; }

        [Option(shortName: 'f',
            longName: "force",
            Required = false,
            HelpText = "Used to force generation of config file when one already exists")]
        public bool Force { get; set; }

        [Option(shortName: 'i',
             longName: "init",
             Required = false,
             HelpText = "Generate default .tsqllintrc config file.")]
        public bool Init { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> LintPath { get; set; }

        [Option(shortName: 'p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to config file"),
        TSQLLINTOption(NonLintingCommand = true)]
        public bool PrintConfig { get; set; }

        [Option(shortName: 'v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version."),
        TSQLLINTOption(NonLintingCommand = true)]
        public bool Version { get; set; }

        [Option(shortName: 'h',
            longName: "help",
            Required = false,
            HelpText = "Display tsqllint version.")]
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