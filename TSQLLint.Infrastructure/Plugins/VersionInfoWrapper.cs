using System.Diagnostics;
using System.Reflection;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Plugins
{
    public class VersionInfoWrapper : IFileversionWrapper
    {
        public string GetVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }
    }
}
