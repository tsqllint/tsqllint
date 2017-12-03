using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins;
using TSQLLint.Lib.Plugins.Interfaces;

namespace TSQLLint.Lib.Parser
{
    public class SqlFileProcessor : ISqlFileProcessor
    {
        private readonly IRuleVisitor _ruleVisitor;
        private readonly IReporter _reporter;
        private readonly IFileSystem _fileSystem;
        private readonly IPluginHandler _pluginHandler;
        
        public int FileCount { get; private set; }

        public SqlFileProcessor(IRuleVisitor ruleVisitor, IPluginHandler pluginHandler, IReporter reporter, IFileSystem fileSystem)
        {
            _ruleVisitor = ruleVisitor;
            _pluginHandler = pluginHandler;
            _reporter = reporter;
            _fileSystem = fileSystem;
        }

        public void ProcessList(List<string> filePaths)
        {
            foreach (var path in filePaths)
            {
                ProcessPath(path);
            }
        }

        private void ProcessFile(string filePath)
        {
            using (var fileStream = GetFileContents(filePath))
            {
                ProcessRules(fileStream, filePath);
                ProcessPlugins(fileStream, filePath);
            }

            FileCount++;
        }

        public void ProcessPath(string path)
        {
            // remove quotes from filePaths
            path = path.Replace("\"", "");

            var filePathList = path.Split(',');
            for (var index = 0; index < filePathList.Length; index++)
            {
                // remove leading and trailing whitespace
                filePathList[index] = filePathList[index].Trim();
            }

            foreach (var filePath in filePathList)
            {
                if (!_fileSystem.File.Exists(filePath))
                {
                    if (_fileSystem.Directory.Exists(filePath))
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

        private void ProcessDirectory(string path)
        {
            var subDirectories = _fileSystem.Directory.GetDirectories(path);
            foreach (var t in subDirectories)
            {
                ProcessPath(t);
            }

            var fileEntries = _fileSystem.Directory.GetFiles(path);
            foreach (var t in fileEntries)
            {
                ProcessIfSqlFile(t);
            }
        }

        private void ProcessIfSqlFile(string fileName)
        {
            if (_fileSystem.Path.GetExtension(fileName).Equals(".sql", StringComparison.InvariantCultureIgnoreCase))
            {
                ProcessFile(fileName);
            }            
        }

        private void ProcessWildCard(string filePath)
        {
            var containsWildCard = filePath.Contains("*") || filePath.Contains("?");
            if (!containsWildCard)
            {
                _reporter.Report($"{filePath} is not a valid file path.");
                return;
            }

            var dirPath = _fileSystem.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = _fileSystem.Directory.GetCurrentDirectory();
            }

            if (!_fileSystem.Directory.Exists(dirPath))
            {
                _reporter.Report($"Directory does not exist: {dirPath}");
                return;
            }

            var searchPattern = _fileSystem.Path.GetFileName(filePath);
            var files = _fileSystem.Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                ProcessIfSqlFile(file);
            }
        }

        private void ProcessRules(Stream fileStream, string filePath)
        {
            _ruleVisitor.VisitRules(filePath, fileStream);
        }

        private void ProcessPlugins(Stream fileStream, string filePath)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            TextReader textReader = new StreamReader(fileStream);
            _pluginHandler.ActivatePlugins(new PluginContext(filePath, textReader));
        }

        private Stream GetFileContents(string filePath)
        {
            return _fileSystem.File.OpenRead(filePath);
        }
    }
}
