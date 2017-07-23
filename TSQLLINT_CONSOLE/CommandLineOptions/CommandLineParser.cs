using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE.CommandLineOptions
{
    public class ConsoleCommandLineOptionParser
    {
        public bool PerformLinting;

        public ConsoleCommandLineOptionParser(string[] args)
        {
            Parser.Default.ParseArgumentsStrict(args, this);
        }

        private string _ConfigFile;

        [Option(shortName: 'c', 
            longName: "config", 
            Required = false,
            HelpText = "Used to specify a .tsqllintrc file path to use rather than the default.")]
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

        [Option(shortName: 'i',
            longName: "init",
            Required = false,
            HelpText = "Generate .tsqllintrc config file.")]
        public bool Init { get; set; }

        [Option(shortName: 'f',
            longName: "files",
            Required = false,
            HelpText = "Target for linting. May contain a single file, a directory to be recursively iterated over, or a list of comma separated files.")]
        public string LintPath { get; set; }

        [Option(shortName: 'p',
            longName: "print-config",
            Required = false,
            HelpText = "Print path to default .tsqllintrc config file")]
        public bool PrintConfig { get; set; }

        [Option(shortName: 'v',
            longName: "version",
            Required = false,
            HelpText = "Display tsqllint version.")]
        public bool Version { get; set; }

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
            help.AddPreOptionsLine("Usage: tsqllint [options]");
            help.AddOptions(this);
            return help;
        }
    }
}