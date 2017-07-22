using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.CommandLineParser
{
    internal class OptionsValidator : IValidator<CommandLineParser>
    {
        private readonly IBaseReporter Reporter;

        public OptionsValidator(IBaseReporter reporter)
        {
            Reporter = reporter;
        }

        public bool Validate(CommandLineParser commandLineParser)
        {
            if (commandLineParser.Init || 
                commandLineParser.Version || 
                commandLineParser.PrintConfig || 
                File.Exists(commandLineParser.ConfigFile))
            {
                return true;
            }

            Reporter.Report(string.Format("Config file not found {0}.", commandLineParser.ConfigFile));
            return false;
        }
    }
}