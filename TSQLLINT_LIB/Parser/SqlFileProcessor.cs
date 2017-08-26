using System.Collections.Generic;
using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor ruleVisitor;
        private readonly IBaseReporter reporter;

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IBaseReporter reporter)
        {
            this.ruleVisitor = ruleVisitor;
            this.reporter = reporter;
        }

        public int FileCount { get; set; }

        public int GetFileCount()
        {
            return FileCount;
        }

        public void ProcessPath(string path)
        {
            // remove quotes from path
            path = path.Replace("\"", string.Empty);

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
                        this.reporter.Report(string.Format("\n{0} is not a valid path.", pathString));
                    }
                }
                else
                {
                    ProcessFile(Utility.Utility.GetFileContents(pathString), pathString);
                }
            }
        }

        public void ProcessFile(string fileContents, string filePath)
        {
            var txtRdr = Utility.Utility.CreateTextReaderFromString(fileContents);
            this.ruleVisitor.VisitRules(filePath, txtRdr);
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
                if (Path.GetExtension(fileName) == ".sql")
                {
                    var fileContents = Utility.Utility.GetFileContents(fileName);
                    ProcessFile(fileContents, fileName);
                }
            }
        }
    }
}