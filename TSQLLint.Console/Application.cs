using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Console.CommandLineOptions;
using TSQLLint.Console.CommandLineOptions.Interfaces;
using TSQLLint.Console.Reporters;
using TSQLLint.Console.Reporters.Interfaces;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins;
using TSQLLint.Lib.Plugins.Interfaces;

namespace TSQLLint.Console
{
    public class Application
    {
        private readonly ICommandLineOptionHandler _commandLineOptionHandler;
        private readonly CommandLineOptions.CommandLineOptions _commandLineOptions;
        private readonly IConfigReader _configReader;
        private readonly ISqlFileProcessor _fileProcessor;
        private readonly IReporter _reporter;
        private readonly IConsoleTimer _timer;

        public Application(string[] args, IReporter reporter)
        {
            _timer = new ConsoleTimer();
            _timer.Start();
            
            _reporter = reporter;
            _commandLineOptions = new CommandLineOptions.CommandLineOptions(args);
            _configReader = new ConfigReader(reporter);
            _commandLineOptionHandler = new CommandLineOptionHandler(_commandLineOptions, new ConfigFileGenerator(), _configReader, reporter);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(_configReader, reporter);
            IPluginHandler pluginHandler = new PluginHandler(reporter, _configReader.GetPlugins());
            _fileProcessor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, new FileSystem());
        }

        public void Run()
        {
            _configReader.LoadConfig(_commandLineOptions.ConfigFile);
            _commandLineOptionHandler.HandleCommandLineOptions(_commandLineOptions);
            _fileProcessor.ProcessList(_commandLineOptions.LintPath);
            _reporter.ReportResults(_timer.Stop(), _fileProcessor.FileCount);
        }
    }
}
