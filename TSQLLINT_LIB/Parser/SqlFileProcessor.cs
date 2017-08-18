using System.Collections.Generic;
using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor RuleVisitor;
        private readonly IBaseReporter Reporter;
        public int FileCount;

        public int GetFileCount()
        {
            return FileCount;
        }

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IBaseReporter reporter)
        {
            RuleVisitor = ruleVisitor;
            Reporter = reporter;
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

                if (!File.Exists(pathString))
                {
                    if (Directory.Exists(pathString))
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
                    ProcessFile(Utility.Utility.GetFileContents(pathString), pathString);
                }
            }
        }

        private void ProcessDirectory(string path)
        {
            var subdirectoryEntries = Directory.GetDirectories(path);
            for (var index = 0; index < subdirectoryEntries.Length; index++)
            {
                var subdirectory = subdirectoryEntries[index];
                ProcessPath(subdirectory);
            }

            var fileEntries = Directory.GetFiles(path);
            for (var index = 0; index < fileEntries.Length; index++)
            {
                var fileName = fileEntries[index];
                ProcessIfSqlFile(fileName);
            }
        }

        private void ProcessIfSqlFile(string fileName)
        {
            if (Path.GetExtension(fileName) == ".sql")
            {
                var fileContents = Utility.Utility.GetFileContents(fileName);
                ProcessFile(fileContents, fileName);
            }            
        }

        private void ProcessWildCard(string path)
        {
            var containsWildCard = path.Contains("*") || path.Contains("?");
            if (!containsWildCard)
            {
                Reporter.Report(string.Format("\n{0} is not a valid path.", path));
            }

            var dirPath = Path.GetDirectoryName(path);
            var searchPattern = path;
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = Directory.GetCurrentDirectory();
            }
            else
            {
                searchPattern = Path.GetFileName(path);
            }
            var files = Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                ProcessIfSqlFile(file);
            }
        }

        public void ProcessFile(string fileContents, string filePath)
        {
            var txtRdr = Utility.Utility.CreateTextReaderFromString(fileContents);
            RuleVisitor.VisitRules(filePath, txtRdr);
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
    }
}