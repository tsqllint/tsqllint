namespace TSQLLint.Lib.Config.Interfaces
{
    public interface IConfigFileGenerator
    {
        string GetDefaultConfigRules();

        void WriteConfigFile(string path);
    }
}