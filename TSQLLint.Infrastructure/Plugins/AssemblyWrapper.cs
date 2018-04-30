using System;
using System.Reflection;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Plugins
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
