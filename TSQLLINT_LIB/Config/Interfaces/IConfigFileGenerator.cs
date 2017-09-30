namespace TSQLLINT_LIB.Config.Interfaces
{
    public interface IConfigFileGenerator
    {
        string GetDefaultConfigRules();

        void WriteConfigFile(string path);
    }
}