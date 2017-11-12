using System;
using System.Reflection;

namespace TSQLLint.Lib.Standard.Parser.Interfaces
{
    public interface IAssemblyWrapper
    {
        Assembly LoadFile(string path);

        Type[] GetExportedTypes(Assembly assembly);
    }
}
