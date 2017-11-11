namespace TSQLLint.Console.CommandLineOptions.Interfaces
{
    public interface ICommandLineOptionHandler
    {
        bool HandleCommandLineOptions();

        void HandleCommandLineOptions_Refactored(CommandLineOptions options);
    }
}
