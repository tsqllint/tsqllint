using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Configuration
{
    public class IgnoreListReader : IIgnoreListReader
    {
        public IEnumerable<string> IgnoreList { get; private set; }

        private readonly IReporter reporter;

        private readonly IFileSystem fileSystem;

        private readonly IEnvironmentWrapper environmentWrapper;

        public IgnoreListReader(IReporter reporter)
            : this(reporter, new FileSystem(), new EnvironmentWrapper()) { }

        public IgnoreListReader(IReporter reporter, IFileSystem fileSystem, IEnvironmentWrapper environmentWrapper)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            this.environmentWrapper = environmentWrapper;
        }

        public bool IsIgnoreListLoaded { get; private set; } = false;

        public void LoadIgnoreList(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var envVariableIgnoreListFilePath = environmentWrapper.GetEnvironmentVariable("tsqllintignore");

                if (!string.IsNullOrWhiteSpace(envVariableIgnoreListFilePath))
                {
                    if (fileSystem.File.Exists(envVariableIgnoreListFilePath))
                    {
                        LoadIgnoreListFromFile(envVariableIgnoreListFilePath);
                        return;
                    }
                }

                var localIgnoreListFilePath = fileSystem.Path.Combine(Environment.CurrentDirectory, @".tsqllintignore");
                if (fileSystem.File.Exists(localIgnoreListFilePath))
                {
                    LoadIgnoreListFromFile(localIgnoreListFilePath);
                    return;
                }

                var defaultIgnoreListFilePath = fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintignore");
                if (fileSystem.File.Exists(defaultIgnoreListFilePath))
                {
                    LoadIgnoreListFromFile(defaultIgnoreListFilePath);
                    return;
                }

                IgnoreList = Enumerable.Empty<string>();
                IsIgnoreListLoaded = true;
            }
            else
            {
                if (fileSystem.File.Exists(path))
                {
                    LoadIgnoreListFromFile(path);
                    return;
                }
                else
                {
                    reporter.Report($@"Config file not found: {path}");
                    IgnoreList = Enumerable.Empty<string>();
                }
            }
        }

        void LoadIgnoreListFromFile(string path)
        {
            IgnoreList = fileSystem.File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Trim());
            IsIgnoreListLoaded = true;
        }
    }
}
