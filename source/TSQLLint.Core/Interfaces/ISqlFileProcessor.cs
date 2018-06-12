using System.Collections.Generic;

namespace TSQLLint.Core.Interfaces
{
    public interface ISqlFileProcessor
    {
        int FileCount { get; }

        void ProcessList(List<string> filePaths);

        void ProcessPath(string path);
    }
}
