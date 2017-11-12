using System.Collections.Generic;

namespace TSQLLint.Lib.Standard.Parser.Interfaces
{
    public interface ISqlFileProcessor
    {
        void ProcessList(List<string> path);

        void ProcessPath(string path);

        int FileCount { get; }
    }
}
