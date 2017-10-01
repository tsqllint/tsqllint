using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Console.ConfigHandler.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.ConfigHandler
{
    public class CommandLineOptionHandler
    {
        private readonly CommandLineOptions _commandLineOptions;
        private readonly IConfigFileFinder _configFileFinder;
        private readonly IConfigFileGenerator _configFileGenerator;
        private readonly IBaseReporter _reporter;

        public CommandLineOptionHandler(
            CommandLineOptions commandLineOptions,
            IConfigFileFinder configFileFinder,
            IConfigFileGenerator configFileGenerator,
            IBaseReporter reporter)
        {
            _commandLineOptions = commandLineOptions;
            _configFileFinder = configFileFinder;
            _configFileGenerator = configFileGenerator;
            _reporter = reporter;
        }

        public bool HandleCommandLineOptions()
        {
            var performLinting = true;

            if (_commandLineOptions.Args.Length == 0)
            {
                ReportUsage();
                performLinting = false;
            }

            performLinting &= CheckOptionsForNonLintingActions(_commandLineOptions);

            if (_commandLineOptions.Version)
            {
                ReportVersionInfo(_reporter);
            }

            CheckConfigFile();

            if (_commandLineOptions.PrintConfig)
            {
                HandlePrintConfigOption();
            }

            if (performLinting && _commandLineOptions.LintPath.Count < 1)
            {
                ReportUsage();
                performLinting = false;
            }

            return performLinting;
        }

        private void ReportUsage()
        {
            _reporter.Report(string.Format(_commandLineOptions.GetUsage()));
        }

        private static void ReportVersionInfo(IBaseReporter reporter)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            reporter.Report(string.Format("v{0}", version));
        }

        private static bool CheckOptionsForNonLintingActions(CommandLineOptions commandLineOptions)
        {
            var properties = typeof(CommandLineOptions).GetProperties();
            foreach (var prop in properties)
            {
                var propertyValue = prop.GetValue(commandLineOptions);
                if (propertyValue is bool)
                {
                    var value = (bool)propertyValue;

                    if (!value)
                    {
                        continue;
                    }
                }

                var attributes = prop.GetCustomAttributes(true);
                for (var index = 0; index < attributes.Length; index++)
                {
                    var attribute = attributes[index] as TSQLLINTOption;

                    if (attribute != null && attribute.NonLintingCommand)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void CheckConfigFile()
        {
            var configFile = _commandLineOptions.ConfigFile;
            _commandLineOptions.DefaultConfigRules = null;
            var useInMemoryRules = false;

            if (string.IsNullOrWhiteSpace(configFile))
            {
                _commandLineOptions.ConfigFile = configFile = _configFileFinder.DefaultConfigFileName;
                useInMemoryRules = !(_commandLineOptions.Init || _commandLineOptions.Force);
            }
            else
            {
                _commandLineOptions.ConfigFile = configFile = configFile.Trim();
            }

            var configFileExists = FileExists(configFile);
            if (useInMemoryRules && !configFileExists)
            {
                _commandLineOptions.DefaultConfigRules = _configFileGenerator.GetDefaultConfigRules();
                _commandLineOptions.ConfigFile = null;
            }
            else if (_commandLineOptions.Init && !configFileExists)
            {
                CreateConfigFile(configFile);
            }
            else if (_commandLineOptions.Force)
            {
                CreateConfigFile(configFile);
            }
            else if (!configFileExists)
            {
                _reporter.Report(string.Format(
                    "Existing config file not found at: {0} use the '--init' option to create if one does not exist or the '--force' option to overwrite",
                    configFile));
            }
        }

        private bool FileExists(string path)
        {
            return _configFileFinder.FindFile(path);
        }

        private void CreateConfigFile(string configFile)
        {
            _configFileGenerator.WriteConfigFile(configFile);
        }

        private void HandlePrintConfigOption()
        {
            _reporter.Report(!string.IsNullOrWhiteSpace(_commandLineOptions.DefaultConfigRules)
                ? "Using default config instead of a file"
                : string.Format("Config file found at: {0}", _commandLineOptions.ConfigFile));
        }
    }
}