using System.Diagnostics;
using System.Reflection;

namespace TSQLLint.Lib.Plugins
{
    public class VersionInfoWrapper : IFileversionWrapper
    {
        public string GetVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }
    }
}