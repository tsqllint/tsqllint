using System.IO;
using TSQLLINT_CONSOLE.CommandLineOptions;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.CommandLineParser
{
    internal class OptionsValidator : IValidator<ConsoleCommandLineOptionParser>
    {
        private readonly IBaseReporter Reporter;

        public OptionsValidator(IBaseReporter reporter)
        {
            Reporter = reporter;
        }

        public bool Validate(ConsoleCommandLineOptionParser consoleCommandLineOptionParser)
        {
            if (consoleCommandLineOptionParser.Init || 
                consoleCommandLineOptionParser.Version || 
                consoleCommandLineOptionParser.PrintConfig || 
                File.Exists(consoleCommandLineOptionParser.ConfigFile))
            {
                return true;
            }

            Reporter.Report(string.Format("\nTSQLLINT Config file not found: {0} \nYou may generate it with the '--init' option", consoleCommandLineOptionParser.ConfigFile));
            return false;
        }
    }
}