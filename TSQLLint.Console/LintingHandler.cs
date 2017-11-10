using TSQLLint.Common;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Plugins;

namespace TSQLLint.Console
{
    public class LintingHandler
    {
        private readonly SqlFileProcessor _parser;

        private readonly CommandLineOptions.CommandLineOptions _commandLineOptions;

        public int LintedFileCount { get; private set; }

        public LintingHandler(CommandLineOptions.CommandLineOptions commandLineOptions, IConfigReader configReader, IReporter reporter)
        {
            _commandLineOptions = commandLineOptions;

            var pluginHandler = new PluginHandler(reporter, configReader.GetPlugins());

            var ruleVisitor = new SqlRuleVisitor(configReader, reporter);
            _parser = new SqlFileProcessor(pluginHandler, ruleVisitor, reporter);
        }

        public void Lint()
        {
            _parser.ProcessList(_commandLineOptions.LintPath);
            LintedFileCount = _parser.FileCount;
        }
    }
}
