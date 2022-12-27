using System;
using System.Reflection;

namespace TSQLLint.Core.Interfaces
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFrom(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}
