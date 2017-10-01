namespace TSQLLint.Console.ConfigHandler.Interfaces
{
    public interface IConfigFileFinder
    {
        string DefaultConfigFileName { get; }

        bool FindFile(string configFile);
    }
}