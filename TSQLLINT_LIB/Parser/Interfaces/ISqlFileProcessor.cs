using System.Collections.Generic;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface ISqlFileProcessor
    {
        void ProcessList(List<string> path);

        void ProcessPath(string path);

        void ProcessFile(string fileContents, string filePath);

        int GetFileCount();
    }
}