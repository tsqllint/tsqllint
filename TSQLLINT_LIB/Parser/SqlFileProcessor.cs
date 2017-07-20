using System;
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
            // remove double quotes from path
            path = path.Replace("\"", "");

            var pathStrings = path.Split(',');

            // remove leading and trailing whitespace
            for (var index = 0; index < pathStrings.Length; index++)
            {
                pathStrings[index] = pathStrings[index].Trim();
            }

            foreach (var pathString in pathStrings)
            {
                if (File.Exists(@pathString))
                {
                    ProcessFile(Utility.GetFileContents(pathString), pathString);
                }
                else if (Directory.Exists(@pathString))
                {
                    ProcessDirectory(pathString);
                }
                else
                {
                    Reporter.Report(string.Format("\n\n{0} is not a valid path.", pathString));
                }
            }
        }

        private void ProcessDirectory(string path)
        {
            var subdirectoryEntries = Directory.GetDirectories(path);
            foreach (var subdirectory in subdirectoryEntries)
            {
                ProcessPath(subdirectory);
            }

            var fileEntries = Directory.GetFiles(path);
            foreach (var fileName in fileEntries)
            {
                if (Path.GetExtension(fileName) != ".sql")
                {
                    continue;
                }
                var fileContents = Utility.GetFileContents(fileName);
                ProcessFile(fileContents, fileName);
            }
        }

        public void ProcessFile(string fileContents, string filePath)
        {
            var txtRdr = Utility.CreateTextReaderFromString(fileContents);
            RuleVisitor.VisitRules(filePath, txtRdr);
            FileCount++;
        }
    }
}