using System.Collections.Generic;
using TSQLLint.Common;

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