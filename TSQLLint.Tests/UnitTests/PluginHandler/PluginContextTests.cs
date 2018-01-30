using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Plugins;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class PluginContextTests
    {
        [Test]
        public void Properties_ShouldReturnInitializedValues()
        {
            // arrange
            const string path = @"c:\foo\foo.sql";
            var foo = Substitute.For<TextReader>();

            // act
            var pluginContext = new PluginContext(path, new List<IRuleException>(), foo);

            // assert
            Assert.AreEqual(path, pluginContext.FilePath);
            Assert.AreEqual(foo, pluginContext.FileContents);
        }
    }
}
