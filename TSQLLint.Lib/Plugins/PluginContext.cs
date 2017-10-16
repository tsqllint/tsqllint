using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Lib.Plugins
{
    public class PluginContext : IPluginContext
    {
        public string FilePath { get; }

        public TextReader FileContents { get; }

        public PluginContext(string filePath, TextReader fileContents)
        {
            FilePath = filePath;
            FileContents = fileContents;
        }
    }
}
