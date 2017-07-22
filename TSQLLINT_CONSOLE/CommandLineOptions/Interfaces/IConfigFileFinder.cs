namespace TSQLLINT_CONSOLE.CommandLineOptions
{
    public interface IConfigFileFinder
    {
        bool FindFile(string configFile);
    }
}