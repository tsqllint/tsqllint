namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface ISqlFileProcessor
    {
        int ProcessPath(string path);
        void ProcessFile(string fileContents, string filePath);
    }
}