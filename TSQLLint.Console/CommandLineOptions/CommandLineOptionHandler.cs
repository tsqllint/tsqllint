using System.Diagnostics;
using System.Reflection;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Console.CommandLineOptions
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
            if (_commandLineOptions.Args.Length == 0)
            {
                ReportUsage();
                return false;
            }

            if (!_commandLineOptions.Version)
            {
                return HandleConfigOptions();
            }
            
            ReportVersionInfo(_reporter);
            return false;
        }

        private bool HandleConfigOptions()
        {
            var createdConfig = CheckOptionsForConfigFile();

            if (_commandLineOptions.PrintConfig)
            {
                HandlePrintConfigOption();
                return false;
            }

            if (_commandLineOptions.LintPath.Count >= 1 || _commandLineOptions.ListPlugins)
            {
                return true;
            }

            if (!createdConfig)
            {
                ReportUsage();
            }

            return false;
        }

        private bool CheckOptionsForConfigFile()
        {
            var allowInMemoryRules = false;
            var configFile = _commandLineOptions.ConfigFile;

            if (string.IsNullOrWhiteSpace(configFile))
            {
                _commandLineOptions.ConfigFile = configFile = _configFileFinder.DefaultConfigFileName;
                allowInMemoryRules = !(_commandLineOptions.Init || _commandLineOptions.Force);
            }
            else
            {
                _commandLineOptions.ConfigFile = configFile = configFile.Trim();
            }

            var configFileExists = FileExists(configFile);
            var createdConfigFile = false;
            if (UseInMemoryRules(configFileExists, allowInMemoryRules))
            {
                // Did not create config file
            }
            else if (CreatedConfigFile(configFileExists, configFile))
            {
                createdConfigFile = true;
            }
            else if (!configFileExists)
            {
                _reporter.Report($"Config file not found at: {configFile} use the '--init' option to create if one does not exist or the '--force' option to overwrite");
            }

            return createdConfigFile;
        }

        private bool UseInMemoryRules(bool configFileExists, bool allowInMemoryRules)
        {
            if (!allowInMemoryRules || configFileExists)
            {
                return false;
            }

            _commandLineOptions.DefaultConfigRules = _configFileGenerator.GetDefaultConfigRules();
            _commandLineOptions.ConfigFile = null;
            return true;
        }

        private bool CreatedConfigFile(bool configFileExists, string configFile)
        {
            if (_commandLineOptions.Init && !configFileExists || _commandLineOptions.Force)
            {
                CreateConfigFile(configFile);
                return true;
            }

            return false;
        }
        
        private bool FileExists(string path)
        {
            return _configFileFinder.FindFile(path);
        }

        private static void ReportVersionInfo(IBaseReporter reporter)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            reporter.Report($"v{version}");
        }

        private void ReportUsage()
        {
            _reporter.Report(string.Format(_commandLineOptions.GetUsage()));
        }

        private void CreateConfigFile(string configFile)
        {
            _configFileGenerator.WriteConfigFile(configFile);
        }

        private void HandlePrintConfigOption()
        {
            _reporter.Report(!string.IsNullOrWhiteSpace(_commandLineOptions.DefaultConfigRules)
                ? "Using default config instead of a file"
                : $"Config file found at: {_commandLineOptions.ConfigFile}");
        }
    }
}
