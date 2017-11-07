using System.Collections.Generic;
using System.IO;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface ISqlFileProcessor
    {
        void ProcessList(List<string> path);

        void ProcessPath(string path);

        void ProcessFile(Stream fileStream, string filePath);

        int FileCount { get; }
    }
}
