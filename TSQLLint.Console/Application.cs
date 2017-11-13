using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using CommandLine;
using TSQLLint.Common;
using TSQLLint.Console.Standard.CommandLineOptions;
using TSQLLint.Console.Standard.Interfaces;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins;
using TSQLLint.Lib.Plugins.Interfaces;
using TSQLLint.Lib.Reporters;
using TSQLLint.Lib.Reporters.Interfaces;

namespace TSQLLint.Console.Standard
{
    public class Application
    {
        private readonly ICommandLineOptionHandler _commandLineOptionHandler;
        private readonly IConfigReader _configReader;
        private readonly ISqlFileProcessor _fileProcessor;
        private readonly IReporter _reporter;
        private readonly IConsoleTimer _timer;

        private ICommandLineOptions _options;

        public Application(string[] args, IReporter reporter)
        {
            Debugger.Break();
            _timer = new ConsoleTimer();
            _timer.Start();
            _reporter = reporter;
            Parser.Default.ParseArguments<Options>(args).WithParsed(options => _options = options);
            _commandLineOptionHandler = new CommandLineOptionHandler(_options, new ConfigFileGenerator(), _configReader, reporter);
            _configReader = new ConfigReader(reporter);
            var fragmentBuilder = new FragmentBuilder();
            var ruleVisitorBuilder = new RuleVisitorBuilder(_configReader, _reporter);
            IRuleVisitor ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
            IPluginHandler pluginHandler = new PluginHandler(reporter, _configReader.GetPlugins());
            _fileProcessor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, new FileSystem());
        }

        public void Run()
        {
            _configReader.LoadConfig(_options.ConfigFile);
            _commandLineOptionHandler.HandleCommandLineOptions(_options);
            _fileProcessor.ProcessList(_options.LintPath.ToList());

            if (_fileProcessor.FileCount > 0)
            {
                _reporter.ReportResults(_timer.Stop(), _fileProcessor.FileCount);
            }
        }
    }
}
