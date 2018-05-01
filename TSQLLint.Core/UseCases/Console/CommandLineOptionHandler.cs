using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.DTO;
using TSQLLint.Core.Interfaces;
using TSQLLint.Core.Interfaces.Config.Contracts;
using TSQLLint.Core.UseCases.Console.HandlerStrategies;

namespace TSQLLint.Core.UseCases.Console
{
    public class CommandLineOptionHandler : IRequestHandler<CommandLineRequestMessage, HandlerResponseMessage>
    {
        private readonly IConfigFileGenerator configFileGenerator;
        private readonly IBaseReporter reporter;
        private readonly IConfigReader configReader;
        private readonly IFileSystemWrapper fileSystemWrapper;

        public CommandLineOptionHandler(
            IConfigFileGenerator configFileGenerator,
            IConfigReader configReader,
            IBaseReporter reporter,
            IFileSystemWrapper fileSystemWrapper)
        {
            this.configFileGenerator = configFileGenerator;
            this.configReader = configReader;
            this.reporter = reporter;
            this.fileSystemWrapper = fileSystemWrapper;
        }

        public HandlerResponseMessage Handle(CommandLineRequestMessage request)
        {
            if (request.CommandLineOptions.Args.Length == 0 || request.CommandLineOptions.Help)
            {
                var strategy = new PrintUsageStrategy(reporter);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }
            
            if (request.CommandLineOptions.Version)
            {
                var strategy = new PrintVersionStrategy(reporter);
                return strategy.HandleCommandLineOptions();
            }
            
            if (request.CommandLineOptions.PrintConfig)
            {
                var strategy = new PrintConfigStrategy(reporter, configReader);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }
            
            if (!string.IsNullOrWhiteSpace(request.CommandLineOptions.ConfigFile))
            {
                var strategy = new LoadConfigFileStrategy(reporter, fileSystemWrapper);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }
            
            if (request.CommandLineOptions.Init)
            {
                var strategy = new CreateConfigFileStrategy(reporter, configFileGenerator, fileSystemWrapper);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }
            
            if (request.CommandLineOptions.ListPlugins)
            {
                var strategy = new PrintPluginsStrategy(reporter, configReader);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }
            
            if (!request.CommandLineOptions.LintPath.Any())
            {
                var strategy = new PrintUsageStrategy(reporter);
                return strategy.HandleCommandLineOptions(request.CommandLineOptions);
            }

            return new HandlerResponseMessage(true, true);
        }
    }
}
