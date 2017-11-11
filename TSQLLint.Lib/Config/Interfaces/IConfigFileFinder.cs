namespace TSQLLint.Lib.Config
{
    public interface IConfigFileFinder
    {
        string DefaultConfgigFilePath { get; }

        bool FindFile(string configFile);
    }
}
