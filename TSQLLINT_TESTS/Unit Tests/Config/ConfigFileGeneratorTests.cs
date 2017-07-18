using System.IO;
using NUnit.Framework;
using TSQLLINT_LIB.Config;

namespace TSQLLINT_LIB_TESTS.Unit_Tests.Config
{
    public class ConfigFileGeneratorTests
    {
        [Test]
        public void WriteConfigFile()
        {
            ConfigFileGenerator.WriteConfigFile();
            var fileName = Path.Combine(TestContext.CurrentContext.WorkDirectory, ".tsqllintrc");
            Assert.IsTrue(File.Exists(fileName));
        }
    }
}