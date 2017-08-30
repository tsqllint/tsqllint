using System.Diagnostics;
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

        public bool PerformLinting { get; private set; }

        public void HandleCommandLineOptions()
        {
            if (_commandLineOptions.Args.Length == 0)
            {
                _reporter.Report(string.Format(_commandLineOptions.GetUsage()));
                PerformLinting = false;
            }

            CheckOptionsForNonLintingActions(_commandLineOptions);

            if (_commandLineOptions.Version)
            {
                ReportVersionInfo(_reporter);
            }

            CheckConfigFile();

            if (_commandLineOptions.PrintConfig)
            {
                HandlePrintConfigOption();
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
                if (propertyValue is bool)
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

        private void CheckConfigFile()
        {
            var configFile = _commandLineOptions.ConfigFile;

            if (string.IsNullOrWhiteSpace(configFile))
            {
                _commandLineOptions.ConfigFile = configFile = _configFileFinder.DefaultConfigFileName;
            }
            else
            {
                _commandLineOptions.ConfigFile = configFile = configFile.Trim();
            }
            if (_commandLineOptions.Init && !FileExists(configFile))
            {
                CreateConfigFile(configFile);
            }
            else if (_commandLineOptions.Force)
            {
                CreateConfigFile(configFile);
            }
            else if (!FileExists(configFile))
            {
                _reporter.Report(string.Format("Existing config file not found at: {0} use the '--init' option to create if one does not exist or the '--force' option to overwrite", configFile));
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
            _reporter.Report(string.Format("Config file found at: {0}", _commandLineOptions.ConfigFile));
        }
    }
}