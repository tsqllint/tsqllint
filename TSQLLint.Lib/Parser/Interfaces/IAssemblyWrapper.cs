using System;
using System.Reflection;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFile(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}
