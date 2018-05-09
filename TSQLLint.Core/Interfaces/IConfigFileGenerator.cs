namespace TSQLLint.Core.Interfaces
{
    public interface IConfigFileGenerator
    {
        string GetDefaultConfigRules();

        void WriteConfigFile(string path);
    }
}
