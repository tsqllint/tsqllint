using System;
using System.IO;

namespace TSQLLINT_CONSOLE
{
    internal class OptionsValidator : IValidator<CommandLineParser>
    {
        public bool Validate(CommandLineParser commandLineParser)
        {
            if (!commandLineParser.Init && !File.Exists(commandLineParser.ConfigFile))
            {
                Console.WriteLine("Config file not found {0}.", commandLineParser.ConfigFile);
                return false;
            }

            return true;
        }
    }
}