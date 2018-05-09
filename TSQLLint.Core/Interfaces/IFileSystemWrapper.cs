namespace TSQLLint.Core.Interfaces
{
    public interface IFileSystemWrapper
    {
        bool FileExists(string path);

        bool PathIsValidForLint(string path);

        string CombinePath(params string[] paths);
    }
}
