using System;
using System.Reflection;

namespace TSQLLINT_LIB.Plugins
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFile(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}