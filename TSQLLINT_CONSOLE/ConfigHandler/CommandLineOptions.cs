using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class CommandLineOptions
    {
        private string _configFile;

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
        public string ConfigFile 
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_configFile))
                {
                    return _configFile;
                }

                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                _configFile = Path.Combine(usersDirectory, @".tsqllintrc");
                return _configFile;
            }
            set { _configFile = value; }
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