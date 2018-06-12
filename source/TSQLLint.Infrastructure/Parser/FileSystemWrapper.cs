using System.IO.Abstractions;
using System.Net.Http.Headers;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class FileSystemWrapper : IFileSystemWrapper
    {
        private readonly FileSystem fileSystem;

        public FileSystemWrapper()
        {
            fileSystem = new FileSystem();
        }
        
        public bool FileExists(string path)
        {
            path = RemoveQuotes(path);
            return fileSystem.File.Exists(path);
        }

        public bool PathIsValidForLint(string path)
        {
            path = RemoveQuotes(path);
            if (!fileSystem.File.Exists(path))
            {
                return fileSystem.Directory.Exists(path) || PathContainsWildCard(path);
            }
            return true;
        }

        private static bool PathContainsWildCard(string filePath)
        {
            return filePath.Contains("*") || filePath.Contains("?");
        }

        private string RemoveQuotes(string path)
        {
            return path.Replace("\"", string.Empty);
        }

        public string CombinePath(params string[] paths)
        {
            return fileSystem.Path.Combine(paths);
        }
    }
}
