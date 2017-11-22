using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins.Interfaces;

namespace TSQLLint.Lib.Plugins
{
    public class PluginHandler : IPluginHandler
    {
        private readonly IReporter _reporter;
        private readonly IAssemblyWrapper _assemblyWrapper;
        private readonly IFileSystem _fileSystem;
        private Dictionary<Type, IPlugin> _plugins;

        public PluginHandler(IReporter reporter) : this(reporter, new FileSystem(), new AssemblyWrapper()) { }

        public PluginHandler(IReporter reporter, IFileSystem fileSystem, IAssemblyWrapper assemblyWrapper)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
            _assemblyWrapper = assemblyWrapper;
        }

        public IList<IPlugin> Plugins => _plugins.Values.ToList();

        private Dictionary<Type, IPlugin> List => _plugins ?? (_plugins = new Dictionary<Type, IPlugin>());

        public void ProcessPaths(Dictionary<string, string> pluginPaths)
        {
            foreach (var pluginPath in pluginPaths)
            {
                ProcessPath(pluginPath.Value);
            }
        }

        public void ProcessPath(string path)
        {
            // remove quotes from path
            path = path.Replace("\"", "").Trim();

            if (!_fileSystem.File.Exists(path))
            {
                if (_fileSystem.Directory.Exists(path))
                {
                    LoadPluginDirectory(path);
                }
                else
                {
                    _reporter.Report($"\nFailed to load plugin(s) defined by '{path}'. No file or directory found by that name.\n");
                }
            }
            else
            {
                LoadPlugin(path);
            }
        }

        public void LoadPluginDirectory(string path)
        {
            var subdirectoryEntries = _fileSystem.Directory.GetDirectories(path);
            foreach (var entry in subdirectoryEntries)
            {
                ProcessPath(entry);
            }

            var fileEntries = _fileSystem.Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (var entry in fileEntries)
            {
                LoadPlugin(entry);
            }
        }

        public void LoadPlugin(string assemblyPath)
        {
            var path = _fileSystem.Path.GetFullPath(assemblyPath);
            var dll = _assemblyWrapper.LoadFile(path);

            foreach (var type in _assemblyWrapper.GetExportedTypes(dll))
            {
                if (!type.GetInterfaces().Contains(typeof(IPlugin)))
                {
                    continue;
                }

                if (!List.ContainsKey(type))
                {
                    List.Add(type, (IPlugin)Activator.CreateInstance(type));

                    _reporter.Report($"Loaded plugin '{type.FullName}'");
                }
                else
                {
                    _reporter.Report($"Already loaded plugin with type '{type.FullName}'");
                }
            }
        }

        public void ActivatePlugins(IPluginContext pluginContext)
        {
            foreach (var plugin in List)
            {
                try
                {
                    plugin.Value.PerformAction(pluginContext, _reporter);
                }
                catch (Exception exception)
                {
                    _reporter.Report($"\nThere was a problem with plugin: {plugin.Key}\n\n{exception}");
                    Trace.WriteLine(exception);
                    throw;
                }
            }
        }
    }
}
