using TSQLLint.Core.Interfaces;
using TSQLLint.Core.Interfaces.Config.Contracts;

namespace TSQLLint.Core.DTO
{
    public class CommandLineRequestMessage : IRequest<CommandLineRequestMessage>, IRequest<HandlerResponseMessage>
    {
        public CommandLineRequestMessage(ICommandLineOptions options)
        {
            CommandLineOptions = options;
        }

        public ICommandLineOptions CommandLineOptions { get; }
    }
}
