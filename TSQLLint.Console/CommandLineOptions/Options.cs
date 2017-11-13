using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using TSQLLint.Console.Standard.Interfaces;

namespace TSQLLint.Console.Standard.CommandLineOptions
{
    public class Options : ICommandLineOptions
    {
        [Option('c',
             longName: "config",
             Required = false,
             HelpText = "Used to specify a .tsqllintrc file path other than the default")]
        public string ConfigFile { get; set; }

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

        [Value(0)]
        public IEnumerable<string> LintPath { get; set; }

        [Option('p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to config file")]
        public bool PrintConfig { get; set; }

        [Option('l',
             longName: "list-plugins",
             Required = false,
             HelpText = "List the loaded plugins")]
        public bool ListPlugins { get; set; }

        [Option('v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version")]
        public bool Version { get; set; }

        [Option('h',
            longName: "help",
            Required = false,
            HelpText = "Display this help dialog")]
        public bool Help { get; set; }

        public string GetUsage()
        {
            var help = new HelpText
            {
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("tsqllint [options] [file.sql] | [dir] | [file.sql | dir]");
            //help.AddOptions(this);
            return help;
        }
    }
}
