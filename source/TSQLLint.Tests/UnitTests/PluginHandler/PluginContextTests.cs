using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Plugins;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class PluginContextTests
    {
        [Test]
        public void Properties_ShouldReturnInitializedValues()
        {
            // arrange
            const string path = @"c:\foo\foo.sql";
            var mockFileStream = Substitute.For<TextReader>();
            var ruleExceptions = new List<IRuleException>();

            // act
            var pluginContext = new PluginContext(path, ruleExceptions, mockFileStream);

            // assert
            Assert.AreEqual(path, pluginContext.FilePath);
            Assert.AreEqual(mockFileStream, pluginContext.FileContents);
            Assert.AreEqual(ruleExceptions, pluginContext.RuleExceptions);
        }
    }
}
