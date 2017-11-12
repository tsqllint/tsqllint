namespace TSQLLint.Lib.Standard.Config.Interfaces
{
    public interface IConfigFileGenerator
    {
        string GetDefaultConfigRules();

        void WriteConfigFile(string path);
    }
}
