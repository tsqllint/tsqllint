﻿using TSQLLINT_CONSOLE.ConfigHandler;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_CONSOLE
{
    public class Application
    {
        private readonly string[] Args;
        private readonly IReporter Reporter;
        private readonly ConsoleTimer Timer = new ConsoleTimer();

        public Application(string[] args, IReporter reporter)
        {
            Timer.start();
            Args = args;
            Reporter = reporter;
        }

        public void Run()
        {
            // parse options
            var commandLineOptions = new CommandLineOptions(Args);

            // perform non-linting actions
            var configHandler = new ConfigHandler.ConfigHandler(commandLineOptions, Reporter);
            if (!configHandler.HandleConfigs())
            {
                return;
            }

            // perform lint
            var lintingHandler = new LintingHandler(commandLineOptions, Reporter);
            lintingHandler.Lint();

            Reporter.ReportResults(Timer.stop(), lintingHandler.LintedFileCount);
        }
    }
}