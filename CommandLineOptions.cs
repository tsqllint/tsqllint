using System.Diagnostics;
using CommandLine;
using CommandLine.Text;

namespace TSQLLINT_CONSOLE
{
    internal class CommandLineOptions
    {
        [Option(shortName: 'c', 
            longName: "config", 
            Required = false, 
            HelpText = "Use configuration from this file or shareable config.", 
            DefaultValue = ".tsqllintrc")]
        public string ConfigFile { get; set; }

        [Option(shortName: 'p',
            longName: "path",
            Required = true,
            HelpText = "Target path for linting")]
        public string LintPath { get; set; }

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
    }
}