using System;
using System.Reflection;
using NUnit.Framework;
using TSQLLINT_LIB.Plugins;

namespace TSQLLINT_LIB_TESTS.UnitTests.PluginHandler
{
    public class AssemblyWrapperTests
    {
        public static string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                return Uri.UnescapeDataString(uri.Path);
            }
        }

        [Test]
        public void AssemblyWrapper_LoadFile_ShouldLoadTestAssembly()
        {
            var assemblyWrapper = new AssemblyWrapper();
            var loadedAssembly = assemblyWrapper.LoadFile(AssemblyPath);

            Assert.AreEqual("TSQLLINT_LIB_TESTS.dll", loadedAssembly.ManifestModule.ScopeName);
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