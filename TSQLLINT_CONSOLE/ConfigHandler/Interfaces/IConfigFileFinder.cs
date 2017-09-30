namespace TSQLLINT_CONSOLE.ConfigHandler.Interfaces
{
    public interface IConfigFileFinder
    {
        string DefaultConfigFileName { get; }

        bool FindFile(string configFile);
    }
}