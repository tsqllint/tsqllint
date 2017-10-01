using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Lib.Plugins
{
    public class PluginContext : IPluginContext
    {
        private readonly string _filePath;
        private readonly TextReader _fileContents;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public TextReader FileContents
        {
            get
            {
                return _fileContents;
            }
        }

        public PluginContext(string filePath, TextReader fileContents)
        {
            _filePath = filePath;
            _fileContents = fileContents;
        }
    }
}
