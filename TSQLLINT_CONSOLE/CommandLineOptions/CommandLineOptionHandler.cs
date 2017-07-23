using System;
using System.Diagnostics;
using System.IO;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.CommandLineOptions
{
    public class CommandLineOptionHandler
    {
        public bool PerformLinting = true;

        public void HandleCommandLineOptions(ConsoleCommandLineOptionParser commandLineOptions, 
            IConfigFileFinder configFileFinder,
            IConfigFileGenerator configFileGenerator, 
            IBaseReporter reporter)
        {
            if (commandLineOptions.Init)
            {
                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var configFilePath = Path.Combine(usersDirectory, @".tsqllintrc");

                configFileGenerator.WriteConfigFile(configFilePath);

                PerformLinting = false;
            }

            if (commandLineOptions.Version)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var version = fvi.FileVersion;
                reporter.Report(string.Format("v{0}", version));

                PerformLinting = false;
            }

            if (commandLineOptions.PrintConfig)
            {
                if (!configFileFinder.FindFile(commandLineOptions.ConfigFile))
                {
                    reporter.Report("Default config file not found. You may generate it with the '--init' option");
                    return;
                }

                reporter.Report(string.Format("Default config file found at: {0}", commandLineOptions.ConfigFile));

                PerformLinting = false;
            }
        }
    }
}