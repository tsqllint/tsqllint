using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.CommandLineOptions
{
    public class CommandLineOptions : ICommandLineOptions
    {
        public CommandLineOptions(string[] args)
        {
            Args = args;
            CommandLine.Parser.Default.ParseArgumentsStrict(args, this);
        }

        public string[] Args { get; set; }

        [Option(
            'c',
             longName: "config",
             Required = false,
             HelpText = "Used to specify a .tsqllintrc file path other than the default")]
        public string ConfigFile { get; set; }

        [Option(
            'f',
            longName: "force",
            Required = false,
            HelpText = "Used to force generation of default config file when one already exists")]
        public bool Force { get; set; }

        [Option(
            'x',
            longName: "fix",
            Required = false,
            HelpText = "Used to fix some of the common linting errors if possible")]
        public bool Fix { get; set; }

        [Option(
            'i',
            longName: "init",
            Required = false,
            HelpText = "Generate default .tsqllintrc config file")]
        public bool Init { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> LintPath { get; set; }

        [Option(
            'p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to config file")]
        public bool PrintConfig { get; set; }

        [Option(
            'g',
             longName: "load-plugins",
             Required = false,
             HelpText = "Used to specify plugins to be loaded in a comma-delimited list")]
        public string Plugins { get; set; }

        [Option(
            'l',
             longName: "list-plugins",
             Required = false,
             HelpText = "List the loaded plugins")]
        public bool ListPlugins { get; set; }

        [Option(
            'v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version")]
        public bool Version { get; set; }

        [Option(
            'h',
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
