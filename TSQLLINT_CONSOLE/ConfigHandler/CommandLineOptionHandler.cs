using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TSQLLINT_CONSOLE.ConfigHandler.Interfaces;
using TSQLLINT_LIB.Config.Interfaces;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE.ConfigHandler
{
    public class CommandLineOptionHandler
    {
        public bool PerformLinting = true;

        public void HandleCommandLineOptions(CommandLineOptions commandLineOptions, IConfigFileFinder configFileFinder, IConfigFileGenerator configFileGenerator, IBaseReporter reporter)
        {
            CheckOptionsForNonLintingActions(commandLineOptions);
            var configFileExists = configFileFinder.FindFile(commandLineOptions.ConfigFile);

            if (commandLineOptions.Init)
            {
                CreateDefaultConfigFile(configFileGenerator);
            }

            if (commandLineOptions.Version)
            {
                ReportVersionInfo(reporter);
            }

            if (commandLineOptions.PrintConfig && configFileExists)
            {
                reporter.Report(string.Format("Config file found at: {0}", commandLineOptions.ConfigFile));
            }

            if (!configFileExists)
            {
                reporter.Report("Config file not found. You may generate it with the '--init' option");
                PerformLinting = false;
            }

            if (PerformLinting && string.IsNullOrWhiteSpace(commandLineOptions.LintPath))
            {
                reporter.Report("Linting path not provided. You may provide it with the '-f' option");
                PerformLinting = false;
            }
        }

        private static void ReportVersionInfo(IBaseReporter reporter)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            reporter.Report(string.Format("v{0}", version));
        }

        private static void CreateDefaultConfigFile(IConfigFileGenerator configFileGenerator)
        {
            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var configFilePath = Path.Combine(usersDirectory, @".tsqllintrc");
            configFileGenerator.WriteConfigFile(configFilePath);
        }

        private void CheckOptionsForNonLintingActions(CommandLineOptions commandLineOptions)
        {
            var properties = typeof(CommandLineOptions).GetProperties();
            foreach (var prop in properties)
            {
                if (!PerformLinting)
                {
                    break;
                }

                var propertyValue = prop.GetValue(commandLineOptions);

                if (propertyValue == null)
                {
                    continue;
                }

                var propertyType = propertyValue.GetType();
                if (propertyType == typeof(bool))
                {
                    var value = (bool)propertyValue;

                    if (!value)
                    {
                        continue;
                    }
                }

                var attrs = prop.GetCustomAttributes(true);
                for (var index = 0; index < attrs.Length; index++)
                {
                    var attr = attrs[index];
                    var attrib = attr as TSQLLINTOption;

                    if (attrib != null && attrib.NonLintingCommand)
                    {
                        PerformLinting = false;
                        return;
                    }
                }
            }
        }
    }
}