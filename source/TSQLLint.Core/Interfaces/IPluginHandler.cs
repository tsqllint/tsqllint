using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface IPluginHandler
    {
        IList<IPlugin> Plugins { get; }

        void ProcessPaths(string pluginPaths);

        void ProcessPaths(Dictionary<string, string> pluginPaths);

        void ProcessPath(string path);

        void LoadPluginDirectory(string path);

        void LoadPlugin(string assemblyPath);

        void ActivatePlugins(IPluginContext pluginContext);
    }
}
