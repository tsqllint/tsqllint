using System;
using System.Diagnostics;
using System.IO;
using TSQLLINT_CONSOLE.ConfigHandler.Interfaces;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class CommandLineOptionHandler
    {
        public bool PerformLinting = true;

        public void HandleCommandLineOptions(ConsoleCommandLineOptionParser commandLineOptions, IConfigFileFinder configFileFinder, IConfigFileGenerator configFileGenerator, IBaseReporter reporter)
        {
            if (!string.IsNullOrWhiteSpace(commandLineOptions.ConfigFile) && !File.Exists(commandLineOptions.ConfigFile))
            {
                reporter.Report(string.Format("\nTSQLLINT Config file not found: {0} \nYou may generate it with the '--init' option", commandLineOptions.ConfigFile));
                PerformLinting = false;
                return;
            }

            if (commandLineOptions.Init)
            {
                var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var configFilePath = Path.Combine(usersDirectory, @".tsqllintrc");
                configFileGenerator.WriteConfigFile(configFilePath);

                PerformLinting = false;
                return;
            }

            if (commandLineOptions.Version)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var version = fvi.FileVersion;
                reporter.Report(string.Format("v{0}", version));

                PerformLinting = false;
                return;
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
                return;
            }

            if (string.IsNullOrWhiteSpace(commandLineOptions.LintPath))
            {
                reporter.Report(commandLineOptions.GetUsage());
                PerformLinting = false;
            }
        }
    }
}