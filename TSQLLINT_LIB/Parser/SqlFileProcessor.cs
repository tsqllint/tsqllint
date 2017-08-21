using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor _ruleVisitor;
        private readonly IBaseReporter _reporter;
        private readonly IFileSystem _fileSystem;
        public int FileCount;

        public int GetFileCount()
        {
            return FileCount;
        }

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IBaseReporter reporter)
            : this(ruleVisitor, reporter, new FileSystem())
        {
        }

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IBaseReporter reporter, IFileSystem fileSystem)
        {
            _ruleVisitor = ruleVisitor;
            _reporter = reporter;
            _fileSystem = fileSystem;
        }

        public void ProcessPath(string path)
        {
            // remove quotes from path
            path = path.Replace("\"", "");

            var pathStrings = path.Split(',');

            // remove leading and trailing whitespace
            for (var index = 0; index < pathStrings.Length; index++)
            {
                pathStrings[index] = pathStrings[index].Trim();
            }

            for (var index = 0; index < pathStrings.Length; index++)
            {
                var pathString = pathStrings[index];

                if (!_fileSystem.File.Exists(pathString))
                {
                    if (_fileSystem.Directory.Exists(pathString))
                    {
                        ProcessDirectory(pathString);
                    }
                    else
                    {
                        ProcessWildCard(pathString);
                    }
                }
                else
                {
                    ProcessFile(GetFileContents(pathString), pathString);
                }
            }
        }

        private void ProcessDirectory(string path)
        {
            var subdirectoryEntries = _fileSystem.Directory.GetDirectories(path);
            for (var index = 0; index < subdirectoryEntries.Length; index++)
            {
                var subdirectory = subdirectoryEntries[index];
                ProcessPath(subdirectory);
            }

            var fileEntries = _fileSystem.Directory.GetFiles(path);
            for (var index = 0; index < fileEntries.Length; index++)
            {
                var fileName = fileEntries[index];
                ProcessIfSqlFile(fileName);
            }
        }

        private void ProcessIfSqlFile(string fileName)
        {
            if (_fileSystem.Path.GetExtension(fileName) == ".sql")
            {
                var fileContents = GetFileContents(fileName);
                ProcessFile(fileContents, fileName);
            }            
        }

        private void ProcessWildCard(string path)
        {
            var containsWildCard = path.Contains("*") || path.Contains("?");
            if (!containsWildCard)
            {
                _reporter.Report(string.Format("\n{0} is not a valid path.", path));
            }

            var dirPath = _fileSystem.Path.GetDirectoryName(path);
            var searchPattern = path;
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = _fileSystem.Directory.GetCurrentDirectory();
            }
            else
            {
                searchPattern = _fileSystem.Path.GetFileName(path);
            }
            var files = _fileSystem.Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                ProcessIfSqlFile(file);
            }
        }

        public void ProcessFile(string fileContents, string filePath)
        {
            using (var txtRdr = Utility.Utility.CreateTextReaderFromString(fileContents))
            {
                _ruleVisitor.VisitRules(filePath, txtRdr);
            }
            FileCount++;
        }

        public void ProcessList(List<string> paths)
        {
            for (var index = 0; index < paths.Count; index++)
            {
                var path = paths[index];
                ProcessPath(path);
            }
        }

        private string GetFileContents(string filePath)
        {
            return _fileSystem.File.ReadAllText(filePath);
        }
    }
}