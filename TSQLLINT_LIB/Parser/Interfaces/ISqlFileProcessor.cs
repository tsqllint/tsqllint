namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface ISqlFileProcessor
    {
        void ProcessPath(string path);
        void ProcessFile(string fileContents, string filePath);
    }
}