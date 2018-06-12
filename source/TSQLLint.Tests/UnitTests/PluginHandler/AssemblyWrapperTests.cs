using System;
using System.Reflection;
using NUnit.Framework;
using TSQLLint.Infrastructure.Plugins;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class AssemblyWrapperTests
    {
        public static string AssemblyPath
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                return Uri.UnescapeDataString(uri.Path);
            }
        }

        [Test]
        public void AssemblyWrapper_LoadFile_ShouldLoadTestAssembly()
        {
            var assemblyWrapper = new AssemblyWrapper();
            var loadedAssembly = assemblyWrapper.LoadFile(AssemblyPath);

            Assert.AreEqual("TSQLLint.Tests.dll", loadedAssembly.ManifestModule.ScopeName);
        }

        [Test]
        public void AssemblyWrapper_GetExportedTypes_ShouldReturnTypes()
        {
            var assemblyWrapper = new AssemblyWrapper();
            var loadedAssembly = assemblyWrapper.LoadFile(AssemblyPath);
            var loadedTypes = assemblyWrapper.GetExportedTypes(loadedAssembly);

            Assert.IsTrue(loadedTypes.Length > 0);
        }
    }
}
