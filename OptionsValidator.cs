using System;
using System.IO;

namespace TSQLLINT
{
    internal class OptionsValidator : IValidator<CommandLineOptions>
    {
        public bool Validate(CommandLineOptions commandLineOptions)
        {
            if (!File.Exists(commandLineOptions.ConfigFile))
            {
                Console.WriteLine("Config file not found {0}.", commandLineOptions.ConfigFile);
                return false;
            }

            return true;
        }
    }
}