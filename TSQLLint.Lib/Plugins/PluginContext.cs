using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Lib.Plugins
{
    public class PluginContext : IPluginContext
    {
        public PluginContext(string filePath, TextReader fileContents)
        {
            FilePath = filePath;
            FileContents = fileContents;
        }

        public string FilePath { get; }

        public TextReader FileContents { get; }
    }
}
