using CommandLine;
using CommandLine.Text;
using System;
using System.Diagnostics;
using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.CommandLineParser
{
    public class CommandLineParser
    {
        private readonly string[] Args;
        private IBaseReporter Reporter;

        public bool PerformLinting;

        public CommandLineParser(string[] args, IBaseReporter reporter)
        {
            Args = args;
            Reporter = reporter;
            PerformLinting = ParseCommandLineOptions();
        }

        private string _ConfigFile;

        [Option(shortName: 'c', 
            longName: "config", 
            Required = false,
            HelpText = "Path to config file")]
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

        [Option(shortName: 'p',
            longName: "path",
            Required = false,
            HelpText = "Target path for linting, or list of one or more files separated by whitespace")]
        public string LintPath { get; set; }

        [Option(shortName: 'i',
            longName: "init",
            Required = false,
            HelpText = "Generate .tsqllintrc file")]
        public bool Init { get; set; }

        private bool ParseCommandLineOptions()
        {
            if (Args == null || Args.Length == 0)
            {
                Reporter.Report(GetUsage());
                return false;
            }

            Parser.Default.ParseArgumentsStrict(Args, this);

            IValidator<CommandLineParser> optionsValidator = new OptionsValidator(Reporter);
            var optionsValid = optionsValidator.Validate(this);

            if (!optionsValid)
            {
                Reporter.Report(GetUsage());
                return false;
            }

            if (Init)
            {
                return false;
            }

            if (string.IsNullOrEmpty(LintPath))
            {
                Reporter.Report(GetUsage());
                return false;
            }

            return true;
        }

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