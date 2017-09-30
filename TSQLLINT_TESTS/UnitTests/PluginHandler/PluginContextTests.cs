using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLINT_LIB.Plugins;

namespace TSQLLINT_LIB_TESTS.UnitTests.PluginHandler
{
    public class PluginContextTests
    {
        [Test]
        public void Properties_ShouldReturnInitializedValues()
        {
            // arrange
            var path = @"c:\foo\foo.sql";
            var foo = Substitute.For<TextReader>();

            // act
            var pluginContext = new PluginContext(path, foo);

            // assert
            Assert.AreEqual(path, pluginContext.FilePath);
            Assert.AreEqual(foo, pluginContext.FileContents);
        }
    }
}