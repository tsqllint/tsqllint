using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.Rules.RuleExceptions;

namespace TSQLLint.Infrastructure.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor ruleVisitor;

        private readonly IReporter reporter;

        private readonly IFileSystem fileSystem;

        private readonly IPluginHandler pluginHandler;

        private readonly IRuleExceptionFinder ruleExceptionFinder;

        private readonly SemaphoreSlim processingLock;

        private int fileCount;

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IPluginHandler pluginHandler, IReporter reporter, IFileSystem fileSystem, int maxThreads)
        {
            this.ruleVisitor = ruleVisitor;
            this.pluginHandler = pluginHandler;
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            ruleExceptionFinder = new RuleExceptionFinder();
            System.Console.WriteLine($"Linting with {maxThreads} threads");
            processingLock = new SemaphoreSlim(maxThreads, maxThreads);
        }

        public int FileCount => fileCount;

        public void ProcessList(List<string> filePaths)
        {

            Parallel.ForEach(filePaths, ProcessPath);
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

            Parallel.ForEach(filePathList, filePath =>
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
            });
        }

        private void ProcessFile(string filePath)
        {
            processingLock.Wait();
            using (var fileStream = GetFileContents(filePath))
            {
                HandleProcessing(filePath, fileStream);
            }
            processingLock.Release();
            Interlocked.Increment(ref fileCount);
        }

        private bool IsWholeFileIgnored(string filePath, IEnumerable<IExtendedRuleException> ignoredRules)
        {
            var ignoredRulesEnum = ignoredRules.ToArray();
            if (!ignoredRulesEnum.Any())
            {
                return false;
            }

            var lineOneRuleIgnores = ignoredRulesEnum.OfType<GlobalRuleException>().Where(x => 1 == x.StartLine).ToArray();
            if (!lineOneRuleIgnores.Any())
            {
                return false;
            }

            var lineCount = 0;
            using (var reader = new StreamReader(GetFileContents(filePath)))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }

            return lineOneRuleIgnores.Any(x => x.EndLine == lineCount);
        }

        private void HandleProcessing(string filePath, Stream fileStream)
        {
            var ignoredRules = ruleExceptionFinder.GetIgnoredRuleList(fileStream).ToList();
            if (IsWholeFileIgnored(filePath, ignoredRules))
            {
                return;
            }
            ProcessRules(fileStream, ignoredRules, filePath);
            ProcessPlugins(fileStream, ignoredRules, filePath);
        }

        private void ProcessDirectory(string path)
        {
            var subDirectories = fileSystem.Directory.GetDirectories(path);
            foreach (var filePath in subDirectories)
            {
                ProcessPath(filePath);
            }

            var fileEntries = fileSystem.Directory.GetFiles(path);
            foreach (var file in fileEntries)
            {
                ProcessIfSqlFile(file);
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
