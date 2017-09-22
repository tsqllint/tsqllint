using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE
{
    public class Application
    {
        private readonly string[] _args;
        private readonly IReporter _reporter;
        private readonly ConsoleTimer _timer = new ConsoleTimer();

        public Application(string[] args, IReporter reporter)
        {
            _timer.start();
            _args = args;
            _reporter = reporter;
        }

        public void Run()
        {
            // parse options
            var commandLineOptions = new CommandLineOptions(_args);

            // perform non-linting actions
            var configHandler = new ConfigHandler.ConfigHandler(commandLineOptions, _reporter);
            if (!configHandler.HandleConfigs())
            {
                return;
            }

            // perform lint
            var lintingHandler = new LintingHandler(commandLineOptions, _reporter);
            lintingHandler.Lint();

            _reporter.ReportResults(_timer.stop(), lintingHandler.LintedFileCount);
        }
    }
}