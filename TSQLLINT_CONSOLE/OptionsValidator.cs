using System;
using System.IO;

namespace TSQLLINT_CONSOLE
{
    internal class OptionsValidator : IValidator<CommandLineOptions>
    {
        public bool Validate(CommandLineOptions commandLineOptions)
        {
            if (!commandLineOptions.Init && !File.Exists(commandLineOptions.ConfigFile))
            {
                Console.WriteLine("Config file not found {0}.", commandLineOptions.ConfigFile);
                return false;
            }

            return true;
        }
    }
}