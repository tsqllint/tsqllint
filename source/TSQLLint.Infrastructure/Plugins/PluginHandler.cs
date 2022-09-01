using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Plugins
{
    public class PluginHandler : IPluginHandler
    {
        private readonly IAssemblyWrapper assemblyWrapper;
        private readonly IFileSystem fileSystem;
        private readonly IReporter reporter;
        private readonly IFileversionWrapper versionWrapper;
        private Dictionary<Type, IPlugin> plugins;
        private Dictionary<string, ISqlLintRule> rules;

        public PluginHandler(IReporter reporter, Dictionary<string, ISqlLintRule> rules)
            : this(reporter, new FileSystem(), new AssemblyWrapper(), new VersionInfoWrapper(), rules) { }

        public PluginHandler(
            IReporter reporter,
            IFileSystem fileSystem,
            IAssemblyWrapper assemblyWrapper,
            IFileversionWrapper versionWrapper,
            Dictionary<string, ISqlLintRule> rules)
        {
            this.reporter = reporter;
            this.fileSystem = fileSystem;
            this.assemblyWrapper = assemblyWrapper;
            this.versionWrapper = versionWrapper;
            this.rules = rules;
        }

        public IList<IPlugin> Plugins => plugins.Values.ToList();

        private Dictionary<Type, IPlugin> List => plugins ??= new Dictionary<Type, IPlugin>();

        public void ProcessPaths(Dictionary<string, string> pluginPaths)
        {
            // process user specified plugins
            foreach (var pluginPath in pluginPaths)
            {
                ProcessPath(pluginPath.Value);
            }
        }

        public void ProcessPath(string path)
        {
            // remove quotes from path
            path = path.Replace("\"", string.Empty).Trim();

            char[] arrToReplace = { '\\', '/' };
            foreach (var toReplace in arrToReplace)
            {
                path = path.Replace(toReplace, fileSystem.Path.DirectorySeparatorChar);
            }

            if (!fileSystem.File.Exists(path))
            {
                if (fileSystem.Directory.Exists(path))
                {
                    LoadPluginDirectory(path);
                }
                else
                {
                    reporter.Report($"\nFailed to load plugin(s) defined by '{path}'. No file or directory found by that name.\n");
                }
            }
            else
            {
                LoadPlugin(path);
            }
        }

        public void LoadPluginDirectory(string path)
        {
            var subdirectoryEntries = fileSystem.Directory.GetDirectories(path);
            foreach (var entry in subdirectoryEntries)
            {
                ProcessPath(entry);
            }

            var fileEntries = fileSystem.Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (var entry in fileEntries)
            {
                LoadPlugin(entry);
            }
        }

        public void LoadPlugin(string assemblyPath)
        {
            var path = fileSystem.Path.GetFullPath(assemblyPath);
            var dll = assemblyWrapper.LoadFile(path);

            foreach (var type in assemblyWrapper.GetExportedTypes(dll))
            {
                var inerfaces = type.GetInterfaces();

                if (!inerfaces.Contains(typeof(IPlugin)))
                {
                    continue;
                }

                if (!List.ContainsKey(type))
                {
                    var plugin = (IPlugin)Activator.CreateInstance(type);
                    List.Add(type, plugin);
                    var version = versionWrapper.GetVersion(dll);
                    reporter.Report($"Loaded plugin: '{type.FullName}', Version: '{version}'");

                    foreach (var rule in plugin.GetRules())
                    {
                        try
                        {
                            rules.Add(rule.Key, rule.Value);
                        }
                        catch (Exception exception)
                        {
                            reporter.Report($"There was a problem with plugin: {type.FullName} - {exception.Message}");
                            Trace.WriteLine(exception);
                        }
                    }
                }
                else
                {
                    reporter.Report($"Already loaded plugin with type '{type.FullName}'");
                }
            }
        }

        public void ActivatePlugins(IPluginContext pluginContext)
        {
            foreach (var plugin in List)
            {
                try
                {
                    plugin.Value.PerformAction(pluginContext, reporter);
                }
                catch (Exception exception)
                {
                    reporter.Report($"There was a problem with plugin: {plugin.Key} - {exception.Message}");
                    Trace.WriteLine(exception);
                }
            }
        }
    }
}
