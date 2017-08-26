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
        private readonly CommandLineOptions _commandLineOptions;
        private readonly IConfigFileFinder _configFileFinder;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IBaseReporter _reporter;

        public CommandLineOptionHandler(CommandLineOptions commandLineOptions, 
            IConfigFileFinder configFileFinder, 
            IConfigFileGenerator configFileGenerator, 
            IBaseReporter reporter)
        {
            PerformLinting = true;
            _commandLineOptions = commandLineOptions;
            _configFileFinder = configFileFinder;
            _configFileGenerator = configFileGenerator;
            _reporter = reporter;
        }

        public bool PerformLinting { get; set; }

        public void HandleCommandLineOptions()
        {
            if (_commandLineOptions.Args.Length == 0)
            {
                _reporter.Report(string.Format(_commandLineOptions.GetUsage()));
                PerformLinting = false;
            }

            CheckOptionsForNonLintingActions(_commandLineOptions);
            var configFileExists = _configFileFinder.FindFile(_commandLineOptions.ConfigFile);

            if (_commandLineOptions.Init)
            {
                HandleInitOptions();
            }

            if (_commandLineOptions.Version)
            {
                ReportVersionInfo(_reporter);
            }

            if (_commandLineOptions.PrintConfig)
            {
                HandlePrintConfigOption(configFileExists);
            }

            if (PerformLinting && !configFileExists)
            {
                _reporter.Report("Config file not found. You may generate it with the '--init' option");
                PerformLinting = false;
            }

            if (PerformLinting && _commandLineOptions.LintPath.Count < 1)
            {
                _reporter.Report(_commandLineOptions.GetUsage());
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

        private void HandlePrintConfigOption(bool configFileExists)
        {
            if (configFileExists)
            {
                _reporter.Report(string.Format("Config file found at: {0}", _commandLineOptions.ConfigFile));
            }
            else
            {
                _reporter.Report("Config file not found. You may generate it with the '--init' option");
            }
        }

        private void HandleInitOptions()
        {
            var usersDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var defaultConfigFile = Path.Combine(usersDirectory, @".tsqllintrc");
            var defaultConfigFileExists = _configFileFinder.FindFile(defaultConfigFile);

            if (!defaultConfigFileExists || _commandLineOptions.Force)
            {
                _configFileGenerator.WriteConfigFile(defaultConfigFile);
            }
            else
            {
                _reporter.Report(string.Format("Existing config file found at: {0} use the '--force' option to overwrite", defaultConfigFile));
            }
        }

        private void CheckOptionsForNonLintingActions(CommandLineOptions commandLineOptions)
        {
            var properties = typeof(CommandLineOptions).GetProperties();
            foreach (var prop in properties)
            {
                if (!PerformLinting)
                {
                    return;
                }

                var propertyValue = prop.GetValue(commandLineOptions);
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