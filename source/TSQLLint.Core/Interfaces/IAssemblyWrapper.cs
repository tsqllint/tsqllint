using System;
using System.Reflection;

namespace TSQLLint.Core.Interfaces
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFile(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}
