using TSQLLint.Core.DTO;

namespace TSQLLint.Core.Interfaces
{
    public interface IHandlingStrategy
    {
        HandlerResponseMessage HandleCommandLineOptions(ICommandLineOptions commandLineOptions);
    }
}
