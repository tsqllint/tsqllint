using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;

namespace TSQLLint.Lib.Plugins
{
    public class PluginHandler : IPluginHandler
    {
        private readonly IAssemblyWrapper _assemblyWrapper;
        private readonly IFileSystem _fileSystem;
        private List<IPlugin> _plugins;
        private IReporter _reporter;

        public IList<IPlugin> Plugins
        {
            get
            {
                return _plugins ?? (_plugins = new List<IPlugin>());
            }
        }

        public PluginHandler(IReporter reporter, Dictionary<string, string> pluginPaths) : this(reporter, pluginPaths, new FileSystem(), new AssemblyWrapper())
        {
        }

        public PluginHandler(IReporter reporter, Dictionary<string, string> pluginPaths, IFileSystem fileSystem, IAssemblyWrapper assemblyWrapper)
        {
            _reporter = reporter;
            _fileSystem = fileSystem;
            _assemblyWrapper = assemblyWrapper;

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
            }
            else
            {
                LoadPlugin(path);
            }
        }

        public void LoadPluginDirectory(string path)
        {
            var subdirectoryEntries = _fileSystem.Directory.GetDirectories(path);
            for (var index = 0; index < subdirectoryEntries.Length; index++)
            {
                ProcessPath(subdirectoryEntries[index]);
            }

            var fileEntries = _fileSystem.Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            for (var index = 0; index < fileEntries.Length; index++)
            {
                LoadPlugin(fileEntries[index]);
            }
        }

        public void LoadPlugin(string assemblyPath)
        {
            var DLL = _assemblyWrapper.LoadFile(assemblyPath);

            foreach (var type in _assemblyWrapper.GetExportedTypes(DLL))
            {
                if (!type.GetInterfaces().Contains(typeof(IPlugin)))
                {
                    continue;
                }

                // todo dont allow duplicates
                Plugins.Add((IPlugin)Activator.CreateInstance(type));
            }
        }

        public void ActivatePlugins(IPluginContext pluginContext)
        {
            for (var index = 0; index < Plugins.Count; index++)
            {
                try
                {
                    Plugins[index].PerformAction(pluginContext, _reporter);
                }
                catch (Exception e)
                {
                    _reporter.Report(String.Format("\nThere was a problem with plugin: {0}\n\n{1}", Plugins[index].GetType(), e));
                    throw;
                }
            }
        }
    }
}
