using System;
using System.Collections.Concurrent;
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

        private ConcurrentDictionary<string, Stream> fileStreams = new ConcurrentDictionary<string, Stream>();

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IPluginHandler pluginHandler, IReporter reporter, IFileSystem fileSystem)
        {
            this.ruleVisitor = ruleVisitor;
            this.pluginHandler = pluginHandler;
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            ruleExceptionFinder = new RuleExceptionFinder();
        }

        private int _fileCount;
        public int FileCount
        {
            get
            {
                return _fileCount;
            }
        }

        public void ProcessList(List<string> filePaths)
        {
            Parallel.ForEach(filePaths, (path) =>
            {
                processPath(path);
            });

            foreach (var sqlFile in fileStreams)
            {
                HandleProcessing(sqlFile.Key, sqlFile.Value);
                sqlFile.Value.Dispose();
            }
        }

        public void ProcessPath(string path)
        {
            processPath(path);
            foreach (var sqlFile in fileStreams)
            {
                HandleProcessing(sqlFile.Key, sqlFile.Value);
                sqlFile.Value.Dispose();
            }
        }

        private void processPath(string path)
        {
            // remove quotes from filePaths
            path = path.Replace("\"", string.Empty);

            var filePathList = path.Split(',');
            for (var index = 0; index < filePathList.Length; index++)
            {
                // remove leading and trailing whitespace
                filePathList[index] = filePathList[index].Trim();
            }

            Parallel.ForEach(filePathList, (filePath) =>
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
            var fileStream = GetFileContents(filePath);
            AddToProcessing(filePath, fileStream);
            
            Interlocked.Increment(ref _fileCount);
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

        private void AddToProcessing(string filePath, Stream fileStream)
        {
            fileStreams.TryAdd(filePath, fileStream);
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
            Parallel.ForEach(subDirectories, (filePath) =>
            {
                processPath(filePath);
            });
            
            var fileEntries = fileSystem.Directory.GetFiles(path);
            Parallel.ForEach(fileEntries, (file) =>
            {
                ProcessIfSqlFile(file);
            });
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
            Parallel.ForEach(files, (file) =>
            {
                ProcessIfSqlFile(file);
            });
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
