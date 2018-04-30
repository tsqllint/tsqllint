using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.RuleExceptions;

namespace TSQLLint.Infrastructure
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor ruleVisitor;

        private readonly IReporter reporter;

        private readonly IFileSystem fileSystem;

        private readonly IPluginHandler pluginHandler;

        private readonly IRuleExceptionFinder ruleExceptionFinder;

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IPluginHandler pluginHandler, IReporter reporter, IFileSystem fileSystem)
        {
            this.ruleVisitor = ruleVisitor;
            this.pluginHandler = pluginHandler;
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            ruleExceptionFinder = new RuleExceptionFinder();
        }

        public int FileCount { get; private set; }

        public void ProcessList(List<string> filePaths)
        {
            foreach (var path in filePaths)
            {
                ProcessPath(path);
            }
        }

        public void ProcessPath(string path)
        {
            // remove quotes from filePaths
            path = path.Replace("\"", string.Empty);

            var filePathList = path.Split(',');
            for (var index = 0; index < filePathList.Length; index++)
            {
                // remove leading and trailing whitespace
                filePathList[index] = filePathList[index].Trim();
            }

            foreach (var filePath in filePathList)
            {
                if (!fileSystem.File.Exists(filePath))
                {
                    if (fileSystem.Directory.Exists(filePath))
                    {
                        ProcessDirectory(filePath);
                    }
                    else
                    {
                        ProcessWildCard(filePath);
                    }
                }
                else
                {
                    ProcessFile(filePath);
                }
            }
        }

        private void ProcessFile(string filePath)
        {
            using (var fileStream = GetFileContents(filePath))
            {
                var ignoredRules = ruleExceptionFinder.GetIgnoredRuleList(fileStream).ToList();
                ProcessRules(fileStream, ignoredRules, filePath);
                ProcessPlugins(fileStream, ignoredRules, filePath);
            }

            reporter.ReportFileResults();
            FileCount++;
        }

        private void ProcessDirectory(string path)
        {
            var subDirectories = fileSystem.Directory.GetDirectories(path);
            foreach (var t in subDirectories)
            {
                ProcessPath(t);
            }

            var fileEntries = fileSystem.Directory.GetFiles(path);
            foreach (var t in fileEntries)
            {
                ProcessIfSqlFile(t);
            }
        }

        private void ProcessIfSqlFile(string fileName)
        {
            if (fileSystem.Path.GetExtension(fileName).Equals(".sql", StringComparison.InvariantCultureIgnoreCase))
            {
                ProcessFile(fileName);
            }
        }

        private void ProcessWildCard(string filePath)
        {
            var containsWildCard = filePath.Contains("*") || filePath.Contains("?");
            if (!containsWildCard)
            {
                reporter.Report($"{filePath} is not a valid file path.");
                return;
            }

            var dirPath = fileSystem.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = fileSystem.Directory.GetCurrentDirectory();
            }

            if (!fileSystem.Directory.Exists(dirPath))
            {
                reporter.Report($"Directory does not exist: {dirPath}");
                return;
            }

            var searchPattern = fileSystem.Path.GetFileName(filePath);
            var files = fileSystem.Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                ProcessIfSqlFile(file);
            }
        }

        private void ProcessRules(Stream fileStream, IEnumerable<IRuleException> ignoredRules, string filePath)
        {
            ruleVisitor.VisitRules(filePath, ignoredRules, fileStream);
        }

        private void ProcessPlugins(Stream fileStream, IEnumerable<IRuleException> ignoredRules, string filePath)
        {
            TextReader textReader = new StreamReader(fileStream);
            pluginHandler.ActivatePlugins(new PluginContext(filePath, ignoredRules, textReader));
        }

        private Stream GetFileContents(string filePath)
        {
            return fileSystem.File.OpenRead(filePath);
        }
    }
}
