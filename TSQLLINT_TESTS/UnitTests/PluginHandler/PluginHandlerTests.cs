using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Plugins;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.PluginHandler
{
    class PluginHandlerTests
    {
        [Test]
        public void LoadPlugins_ShouldLoadIPluginsFromPathAndFile()
        {
            //arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
            const string filePath2 = @"c:\pluginDirectory\plugin_two.dll";
            const string filePath3 = @"c:\pluginDirectory\plugin_three.dll";
            const string filePath4 = @"c:\pluginDirectory\foo.txt";
            const string filePath5 = @"c:\pluginDirectory\subDirectory\bar.txt";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData(string.Empty)},
                {filePath2, new MockFileData(string.Empty)},
                {filePath3, new MockFileData(string.Empty)},
                {filePath4, new MockFileData("foo")},
                {filePath5, new MockFileData("bar")},
            });

            var assembly = Assembly.GetExecutingAssembly();

            var assemblyWrapper = Substitute.For<IAssemblyWrapper>();
            assemblyWrapper.LoadFile(Arg.Any<string>()).Returns(assembly);
            assemblyWrapper.GetExportedTypes(assembly).Returns(
                new[]
                {
                    typeof(TestPlugin),
                    typeof(string)
                }
            );

            var reporter = Substitute.For<IReporter>();

            var pluginPaths = new Dictionary<string, string>
            {
                { "my-first-plugin", @"c:\pluginDirectory\" },
                { "my-second-plugin", @"c:\pluginDirectory\plugin_one.dll" }
            };

            //act
            var pluginHandler = new TSQLLINT_LIB.Plugins.PluginHandler(reporter, pluginPaths, fileSystem, assemblyWrapper);

            //assert
            Assert.AreEqual(4, pluginHandler.Plugins.Count);
        }

        [Test]
        public void ActivatePlugins_PluginRuleViolations_ShouldCallReporter()
        {
            //arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData(string.Empty)}
            });

            var assembly = Assembly.GetExecutingAssembly();

            var assemblyWrapper = Substitute.For<IAssemblyWrapper>();
            assemblyWrapper.LoadFile(Arg.Any<string>()).Returns(assembly);
            assemblyWrapper.GetExportedTypes(assembly).Returns(
                new[]
                {
                    typeof(TestPlugin),
                    typeof(string)
                }
            );

            var pluginPaths = new Dictionary<string, string>
            {
                {"my-plugin", filePath1}
            };

            var reporter = Substitute.For<IReporter>();
            var textReader = TSQLLINT_LIB.Utility.Utility.CreateTextReaderFromString("\tSELECT * FROM FOO");
            var context = new PluginContext(@"c:\scripts\foo.sql", textReader);

            //act
            var pluginHandler = new TSQLLINT_LIB.Plugins.PluginHandler(reporter, pluginPaths, fileSystem, assemblyWrapper);

            //assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            Assert.DoesNotThrow(() => pluginHandler.ActivatePlugins(context));
            reporter.Received().ReportViolation(Arg.Is<IRuleViolation>(x => 
                x.FileName == context.FilePath 
                && x.RuleName == "prefer-tabs"
                && x.Text == "Should use spaces rather than tabs"
                && x.Line == 1
                && x.Column == 0
                && x.Severity == RuleViolationSeverity.Warning
            ));
        }

        [Test]
        public void ActivatePlugins_ThrowErrors_ShouldCatch_ShouldReport()
        {
            //arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {filePath1, new MockFileData(string.Empty)}
            });

            var assemblyWrapper = Substitute.For<IAssemblyWrapper>();
            var assembly = Assembly.GetExecutingAssembly();
            assemblyWrapper.LoadFile(Arg.Any<string>()).Returns(assembly);
            assemblyWrapper.GetExportedTypes(assembly).Returns(
                new[]
                {
                    typeof(TestPlugin_ThrowsException)
                }
            );

            var pluginPaths = new Dictionary<string, string>    
            {
                { "my-plugin", filePath1 },
                { "my-plugin-directories", @"c:\pluginDirectory" },
                { "my-plugin-invalid-path", @"c:\doesnt-exist" },

            };

            var reporter = Substitute.For<IReporter>();
            var context = Substitute.For<IPluginContext>();

            //act
            var pluginHandler = new TSQLLINT_LIB.Plugins.PluginHandler(reporter, pluginPaths, fileSystem, assemblyWrapper);

            //assert
            Assert.AreEqual(2, pluginHandler.Plugins.Count);
            Assert.Throws<NotImplementedException>(() => pluginHandler.ActivatePlugins(context));
            reporter.Received().Report(Arg.Any<string>());
        }

        public class TestPlugin_ThrowsException : IPlugin
        {
            public void PerformAction(IPluginContext context, IReporter reporter)
            {
                throw new NotImplementedException();
            }
        }
    }
}
