using System;
using System.IO;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor RuleVisitor;
        public int FileCount;

        public int GetFileCount()
        {
            return FileCount;
        }

        public SqlFileProcessor(IRuleVisitor ruleVisitor)
        {
            RuleVisitor = ruleVisitor;
        }

        public void ProcessPath(string path)
        {
            // remove double quotes from path
            path = path.Replace("\"", "");

            if (File.Exists(path))
            {
                ProcessFile(Utility.GetFileContents(path), path);
                return;
            }

            if (Directory.Exists(path))
            {
                var subdirectoryEntries = Directory.GetDirectories(path);
                foreach (var subdirectory in subdirectoryEntries)
                {
                    ProcessPath(subdirectory);
                }
            }
            else
            {
                Console.WriteLine("{0} is not a valid path.", path);
                return;
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