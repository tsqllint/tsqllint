using System;
using System.Reflection;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Lib.Plugins
{
    public class AssemblyWrapper : IAssemblyWrapper
    {
        public Assembly LoadFile(string path)
        {
            return Assembly.LoadFile(path);
        }

        public Type[] GetExportedTypes(Assembly assembly)
        {
            return assembly.GetExportedTypes();
        }
    }
}
