using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class CommandLineOptions
    {
        public string[] Args;

        public CommandLineOptions(string[] args)
        {
            Args = args;
            Parser.Default.ParseArgumentsStrict(args, this);
        }

        private string _ConfigFile;

        [Option(shortName: 'c',
             longName: "config",
             Required = false,
             HelpText = "Used to specify a .tsqllintrc file path other than the default.")]
        public string ConfigFile {
            get
            {
                if (!string.IsNullOrWhiteSpace(_ConfigFile))
                {
                    return _ConfigFile;
                }

                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                _ConfigFile = Path.Combine(usersDirectory, @".tsqllintrc");
                return _ConfigFile;
            }
            set { _ConfigFile = value; }
        }

        [Option(shortName: 'f',
            longName: "force",
            Required = false,
            HelpText = "Used to force generation of default config file when one already exists")]
        public bool Force { get; set; }

        [Option(shortName: 'i',
            longName: "init",
            Required = false,
            HelpText = "Generate default .tsqllintrc config file."),
        TSQLLINTOption(NonLintingCommand = true)]
        public bool Init { get; set; }

        [ValueList(typeof(List<string>))]
        public List<string> LintPath { get; set; }

        [Option(shortName: 'p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to default .tsqllintrc config file"),
        TSQLLINTOption(NonLintingCommand = true)]
        public bool PrintConfig { get; set; }

        [Option(shortName: 'v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version."),
        TSQLLINTOption(NonLintingCommand = true)]
        public bool Version { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                AddDashesToOption = true,
            };

            help.AddPreOptionsLine("tsqllint [options] file.sql | [file.sql] | [dir] | [file.sql | dir]");
            help.AddOptions(this);
            return help;
        }
    }
}