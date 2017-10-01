using System;
using System.Reflection;

namespace TSQLLint.Lib.Plugins
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFile(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}