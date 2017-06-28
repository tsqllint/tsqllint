using System;
using System.IO;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;

namespace TSQLLINT_LIB.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor RuleVisitor;
        private readonly ILintConfigReader ConfigReader;

        public SqlFileProcessor(IRuleVisitor ruleVisitor, ILintConfigReader configReader)
        {
            RuleVisitor = ruleVisitor;
            ConfigReader = configReader;
        }

        public void ProcessPath(string path)
        {
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
                if (Path.GetExtension(fileName) != ".sql") continue;
                var fileContents = Utility.GetFileContents(fileName);
                ProcessFile(fileContents, fileName);
            }
        }

        public void ProcessFile(string fileContents, string filePath)
        {
            var txtRdr = Utility.CreateTextReaderFromString(fileContents);
            RuleVisitor.VisitRules(ConfigReader, filePath, txtRdr);
        }
    }
}

