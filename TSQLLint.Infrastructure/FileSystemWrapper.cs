using System.IO.Abstractions;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure
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
            return fileSystem.File.Exists(path);
        }

        public string CombinePath(params string[] paths)
        {
            return fileSystem.Path.Combine(paths);
        }
    }
}
