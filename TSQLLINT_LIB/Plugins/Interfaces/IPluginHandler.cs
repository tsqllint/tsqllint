using System.Collections.Generic;
using TSQLLINT_COMMON;

namespace TSQLLINT_LIB.Plugins
{
    public interface IPluginHandler
    {
        IList<IPlugin> Plugins { get; }
        void ProcessPath(string path);
        void LoadPluginDirectory(string path);
        void LoadPlugin(string assemblyPath);
        void ActivatePlugins(IPluginContext pluginContext);
    }
}