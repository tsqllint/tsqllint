namespace TSQLLint.Console.CommandLineOptions.Interfaces
{
    public interface IConfigFileFinder
    {
        string DefaultConfigFileName { get; }

        bool FindFile(string configFile);
    }
}
