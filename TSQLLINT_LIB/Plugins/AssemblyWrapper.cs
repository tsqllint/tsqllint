using System;
using System.Reflection;

namespace TSQLLINT_LIB.Plugins
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